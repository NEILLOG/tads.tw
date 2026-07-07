using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTbAboutContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbAboutContent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IntroNameZh = table.Column<string>(type: "TEXT", nullable: true),
                    IntroNameEn = table.Column<string>(type: "TEXT", nullable: true),
                    IntroSummary = table.Column<string>(type: "TEXT", nullable: true),
                    IntroDetail = table.Column<string>(type: "TEXT", nullable: true),
                    IntroImageFileId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    OrgChartFileId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    BoardTermNo = table.Column<int>(type: "INTEGER", nullable: true),
                    BoardPresident = table.Column<string>(type: "TEXT", nullable: true),
                    BoardVicePresident = table.Column<string>(type: "TEXT", nullable: true),
                    BoardExecDirectors = table.Column<string>(type: "TEXT", nullable: true),
                    BoardDirectors = table.Column<string>(type: "TEXT", nullable: true),
                    BoardExecSupervisor = table.Column<string>(type: "TEXT", nullable: true),
                    BoardSupervisors = table.Column<string>(type: "TEXT", nullable: true),
                    BoardSecretaryGeneral = table.Column<string>(type: "TEXT", nullable: true),
                    BoardExecSecretary = table.Column<string>(type: "TEXT", nullable: true),
                    PastBoardListUrl = table.Column<string>(type: "TEXT", nullable: true),
                    MembershipInviteZh = table.Column<string>(type: "TEXT", nullable: true),
                    MembershipInviteEn = table.Column<string>(type: "TEXT", nullable: true),
                    ConstitutionText = table.Column<string>(type: "TEXT", nullable: true),
                    ConstitutionPdfFileId = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    MemberResearchDesc = table.Column<string>(type: "TEXT", nullable: true),
                    CreateUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyUser = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAboutContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbAboutPricing",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    NameZh = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Price = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DescZh = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAboutPricing", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbAboutContent");

            migrationBuilder.DropTable(
                name: "TbAboutPricing");
        }
    }
}
