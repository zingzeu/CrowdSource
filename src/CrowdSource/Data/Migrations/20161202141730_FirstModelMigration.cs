using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CrowdSource.Data.Migrations
{
    public partial class FirstModelMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateTable(
                name: "AUEGVs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUEGVs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    CollectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.CollectionId);
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.FieldTypeId);
                });

            migrationBuilder.CreateTable(
                name: "GVSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GVSuggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CollectionForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Groups_Collections_CollectionForeignKey",
                        column: x => x.CollectionForeignKey,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    FieldId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FieldMetadata = table.Column<string>(nullable: true),
                    FieldTypeForeignKey = table.Column<int>(nullable: true),
                    GroupForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.FieldId);
                    table.ForeignKey(
                        name: "FK_Fields_FieldTypes_FieldTypeForeignKey",
                        column: x => x.FieldTypeForeignKey,
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fields_Groups_GroupForeignKey",
                        column: x => x.GroupForeignKey,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupVersions",
                columns: table => new
                {
                    GroupVersionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupForeignKey = table.Column<int>(nullable: true),
                    NextVersionGroupVersionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupVersions", x => x.GroupVersionId);
                    table.ForeignKey(
                        name: "FK_GroupVersions_Groups_GroupForeignKey",
                        column: x => x.GroupForeignKey,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupVersions_GroupVersions_NextVersionGroupVersionId",
                        column: x => x.NextVersionGroupVersionId,
                        principalTable: "GroupVersions",
                        principalColumn: "GroupVersionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suggestions",
                columns: table => new
                {
                    SuggestionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserForeignKey = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    FieldForeignKey = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suggestions", x => x.SuggestionId);
                    table.ForeignKey(
                        name: "FK_Suggestions_AspNetUsers_ApplicationUserForeignKey",
                        column: x => x.ApplicationUserForeignKey,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suggestions_Fields_FieldForeignKey",
                        column: x => x.FieldForeignKey,
                        principalTable: "Fields",
                        principalColumn: "FieldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FieldTypeForeignKey",
                table: "Fields",
                column: "FieldTypeForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_GroupForeignKey",
                table: "Fields",
                column: "GroupForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CollectionForeignKey",
                table: "Groups",
                column: "CollectionForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_GroupVersions_GroupForeignKey",
                table: "GroupVersions",
                column: "GroupForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_GroupVersions_NextVersionGroupVersionId",
                table: "GroupVersions",
                column: "NextVersionGroupVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_ApplicationUserForeignKey",
                table: "Suggestions",
                column: "ApplicationUserForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_Suggestions_FieldForeignKey",
                table: "Suggestions",
                column: "FieldForeignKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUEGVs");

            migrationBuilder.DropTable(
                name: "GroupVersions");

            migrationBuilder.DropTable(
                name: "GVSuggestions");

            migrationBuilder.DropTable(
                name: "Suggestions");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "FieldTypes");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
