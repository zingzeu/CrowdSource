using System;
using System.Collections.Generic;
using System.Linq;
using CrowdSource.Services;
using CrowdSource.Data;
using CrowdSource.Models;
using CrowdSource.Models.CoreModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdSource.Models.CoreViewModels;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using CrowdSource.Auth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace CrowdSource.Controllers
{
    [Route("api")]
    public class ApiController : Controller {

        private readonly IHttpContextAccessor _httpContext;
        private readonly ITaskDispatcher _taskDispatcher;
        private readonly IDataLogic _logic;
        private readonly ApplicationDbContext _context;
        private readonly ITextSanitizer _textSanitizer;
        private readonly UserManager<ApplicationUser> _userMan;
        private readonly SignInManager<ApplicationUser> _signinMan;
        public ApiController(IHttpContextAccessor httpContext,
                             ITaskDispatcher taskDispatcher,
                             IDataLogic logic,
                             ApplicationDbContext context,
                             ITextSanitizer textSanitizer,
                             UserManager<ApplicationUser> userMan,
                             SignInManager<ApplicationUser> signinMan) {
            _httpContext = httpContext;
            _taskDispatcher = taskDispatcher;
            _logic = logic;
            _context = context;
            _textSanitizer = textSanitizer;
            _userMan = userMan;
            _signinMan = signinMan;
        }

        public async Task<IActionResult> Home() {
            var userClaims = _httpContext.HttpContext.User;
            Console.WriteLine(JsonConvert.SerializeObject(userClaims, new JsonSerializerSettings() {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            }));
            var currentUser = await _userMan.GetUserAsync(userClaims);
            return new JsonResult(new Dictionary<string, string> {
                {"status", "ok"},
                {"user", currentUser?.Email ?? "NotLoggedIn"}
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> GetAuthToken([FromBody] LoginData data) {
            var result = await _signinMan.PasswordSignInAsync(data.Email, data.Password, false ,false);
            
            if (result.Succeeded) {
                var requestAt = DateTime.Now; 
                var expiresIn = requestAt + TokenAuthOption.ExpiresSpan; 
                var token = GenerateToken(data, expiresIn); 
                // dirty hack: sign it out
               // await _signinMan.SignOutAsync();
                return new JsonResult(new Dictionary<string,string> 
                { 
                    {"status","ok"},
                    {"requertAt", requestAt.ToUniversalTime().ToString()},
                    {"expiresIn",TokenAuthOption.ExpiresSpan.TotalSeconds.ToString()}, 
                    {"tokeyType", TokenAuthOption.TokenType},
                    {"accessToken", token} 
                }); 
            } else {
                return Unauthorized();
            }

        }

        private string GenerateToken(LoginData user, DateTime expires) 
        { 
            var handler = new JwtSecurityTokenHandler(); 

            ClaimsIdentity identity = new ClaimsIdentity( 
                new GenericIdentity(user.Email, "TokenAuth"), 
                new[] { 
                    new Claim("Email", user.Email) 
                } 
            ); 
 
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor 
            { 
                Issuer = TokenAuthOption.Issuer, 
                Audience = TokenAuthOption.Audience, 
                SigningCredentials = TokenAuthOption.SigningCredentials, 
                Subject = identity, 
                Expires = expires 
            }); 
            return handler.WriteToken(securityToken); 
        } 
        
        [Route("GetAssignedGroup")]
        public async Task<IActionResult> GetAssignedGroup() {
            var t = _taskDispatcher.GetNextToDo()?.GroupId ?? -1;
            if ((await _logic.GroupExists(t))) {
                var fields = _logic.GetLastestVersionFields(t);
                var types = _logic.GetAllFieldTypesByGroup(t);
                return new JsonResult(FieldsToGroupViewModel(t,fields,types));
            } else {
                return NotFound();
            }
        }

        [Route("GetSpecificGroup/{id}")]
       // [Authorize(Roles="Administrator")]
        public async Task<IActionResult> GetSpecificGroup(int id = 1) {
            if ((await _logic.GroupExists(id))) {
                var fields = _logic.GetLastestVersionFields(id);
                var types = _logic.GetAllFieldTypesByGroup(id);
                return new JsonResult(FieldsToGroupViewModel(id,fields,types));
            } else {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("SubmitGroup")]
        public async Task<IActionResult> SubmitGroup([FromBody]GroupViewModel data){
            if (data==null) {
                return BadRequest();
            }
            if (!(await _logic.GroupExists(data.GroupId))) {
                return BadRequest();
            }
            var currentUser = await _userMan.GetUserAsync(_httpContext.HttpContext.User);
            var types = _logic.GetAllFieldTypesByGroup(data.GroupId);
            var fields = GroupViewModelToFields(data, types);

            _logic.GroupNewSuggestion(data.GroupId, fields, currentUser);
            return Ok();

        }
        private Dictionary<FieldType,string> GroupViewModelToFields(GroupViewModel data, IEnumerable<FieldType> types)
        {
            var id = data.GroupId;
            var fields = new Dictionary<FieldType, string>();

            fields[types.Single(t => t.Name == "TextBUC")] = _textSanitizer.BanJiao(data.TextBUC?.Trim());
            fields[types.Single(t => t.Name == "TextChinese")] = _textSanitizer.BanJiao(data.TextChinese?.Trim());
            fields[types.Single(t => t.Name == "TextEnglish")] = _textSanitizer.BanJiao(data.TextEnglish?.Trim());
            fields[types.Single(t => t.Name == "IsOral")] = data.IsOral.ToString();
            fields[types.Single(t => t.Name == "IsLiterary")] = data.IsLiterary.ToString();
            fields[types.Single(t => t.Name == "IsPivotRow")] = data.IsPivotRow.ToString();
            fields[types.Single(t => t.Name == "BoPoMoFo")] = _textSanitizer.BanJiao(data.BoPoMoFo?.Trim());
            fields[types.Single(t => t.Name == "Radical")] = _textSanitizer.BanJiao(data.Radical?.Trim());

            return fields;
        }

        private GroupViewModel FieldsToGroupViewModel(int gid, Dictionary<FieldType, string> fields, IEnumerable<FieldType>types)
        {
            return new GroupViewModel()
            {
                GroupId = gid,
                TextBUC = fields[types.Single(t => t.Name == "TextBUC")],
                TextChinese = fields[types.Single(t => t.Name == "TextChinese")],
                TextEnglish = fields[types.Single(t => t.Name == "TextEnglish")],
                IsOral = (fields[types.Single(t => t.Name == "IsOral")] == "True"),
                IsLiterary = (fields[types.Single(t => t.Name == "IsLiterary")] == "True"),
                IsPivotRow = (fields[types.Single(t => t.Name == "IsPivotRow")] == "True"),
                BoPoMoFo = fields[types.Single(t => t.Name == "BoPoMoFo")],
                Radical = fields[types.Single(t => t.Name == "Radical")],
                FlagType = _context.Groups.Single(g => g.GroupId == gid).FlagType,
                ImageUrl = _logic.GetGroupMetadata(gid)["ImgFileName"]
            };
        }

    }


    public class LoginData {
        public string Email;
        public string Password;
    }
}