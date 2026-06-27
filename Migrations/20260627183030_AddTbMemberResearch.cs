using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbMemberResearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbMemberResearch",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileID = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    AuthorInfo = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    IntroTitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IntroContent = table.Column<string>(type: "TEXT", nullable: true),
                    Citation = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AuthorBioName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AuthorBioPosition = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    AuthorBioExpertise = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDelete = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPublish = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbMemberResearch", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbMemberResearch");
        }
    }
}
