using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrowdSource.Data;
using CrowdSource.Models.CoreModels;

namespace CrowdSource.Controllers
{
    [Produces("application/json")]
    [Route("api/FieldsApi")]
    public class FieldsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FieldsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FieldsApi
        [HttpGet]
        public IEnumerable<Field> GetFields()
        {
            return _context.Fields;
        }

        // GET: api/FieldsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetField([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @field = await _context.Fields.SingleOrDefaultAsync(m => m.FieldId == id);

            if (@field == null)
            {
                return NotFound();
            }

            return Ok(@field);
        }

        // PUT: api/FieldsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutField([FromRoute] int id, [FromBody] Field @field)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != @field.FieldId)
            {
                return BadRequest();
            }

            _context.Entry(@field).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FieldsApi
        [HttpPost]
        public async Task<IActionResult> PostField([FromBody] Field @field)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Fields.Add(@field);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FieldExists(@field.FieldId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetField", new { id = @field.FieldId }, @field);
        }

        // DELETE: api/FieldsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var @field = await _context.Fields.SingleOrDefaultAsync(m => m.FieldId == id);
            if (@field == null)
            {
                return NotFound();
            }

            _context.Fields.Remove(@field);
            await _context.SaveChangesAsync();

            return Ok(@field);
        }

        private bool FieldExists(int id)
        {
            return _context.Fields.Any(e => e.FieldId == id);
        }
    }
}