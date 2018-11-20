using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CrowdSource.Data;

namespace CrowdSource.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181119180000_Functions")]
    public class Functions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var count_suggestions = @"CREATE OR REPLACE FUNCTION count_suggestions(_field_type text)
                                    RETURNS TABLE (""GroupId"" int4) AS
                                    $func$
                                        select ""Groups"".""GroupId"" as ""GroupId""
                                    FROM ""Groups""
                                        inner join (
                                            select ""GroupVersions"".""GroupId"" as ""GroupId"", ""FieldTypes"".""Name"" as ""FieldType"" from ""GroupVersions"" 
                                                inner join ""GVSuggestions"" ON ""GVSuggestions"".""GroupVersionForeignKey"" = ""GroupVersions"".""GroupVersionId""
                                                inner JOIN ""FieldTypes"" ON ""GVSuggestions"".""FieldTypeForeignKey"" = ""FieldTypes"".""FieldTypeId""
                                            where ""FieldTypes"".""Name"" = _field_type
                                        ) as ""tmp"" ON ""Groups"".""GroupId"" = ""tmp"".""GroupId""
                                    where
                                        ""Groups"".""FlagType"" IS NULL
                                    $func$ LANGUAGE sql;";

            var score =  @"
                        CREATE OR REPLACE FUNCTION score_group(_has_buc bool, _has_eng bool, _has_chi bool)
                        RETURNS TABLE (""score"" int4) AS
                        $func$
                            select (not _has_buc)::int * 8 + (not _has_eng)::int * 4 + (not _has_chi)::int * 2
                        $func$ LANGUAGE sql;";
            migrationBuilder.Sql(count_suggestions);
            migrationBuilder.Sql(score);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
