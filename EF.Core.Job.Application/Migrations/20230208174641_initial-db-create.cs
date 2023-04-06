using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EF.Core.Job.Application.Migrations
{
    public partial class initialdbcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    JobApplicationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(nullable: true),
                    AgencyName = table.Column<string>(nullable: true),
                    WebURL = table.Column<string>(nullable: true),
                    ContactPersonName = table.Column<string>(maxLength: 100, nullable: false),
                    ContactEmail = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: false),
                    Province = table.Column<string>(nullable: false),
                    AppliedOn = table.Column<DateTime>(nullable: false),
                    AppStatus = table.Column<int>(nullable: false),
                    FollowUpNotes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.JobApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "UserResumeCreate",
                columns: table => new
                {
                    UserResumeCreateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    UserIPAddress = table.Column<string>(nullable: true),
                    ResumeCreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResumeCreate", x => x.UserResumeCreateId);
                });

            migrationBuilder.CreateTable(
                name: "UserResumeEmail",
                columns: table => new
                {
                    UserResumeEmailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    ResumeEmailedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResumeEmail", x => x.UserResumeEmailId);
                });

            migrationBuilder.CreateTable(
                name: "AppStatusLog",
                columns: table => new
                {
                    AppStatusLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppStatus = table.Column<int>(nullable: false),
                    AppStatusChangedOn = table.Column<DateTime>(nullable: false),
                    JobApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStatusLog", x => x.AppStatusLogId);
                    table.ForeignKey(
                        name: "FK_AppStatusLog_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobResumes",
                columns: table => new
                {
                    JobResumeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(nullable: false),
                    JobApplicationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobResumes", x => x.JobResumeId);
                    table.ForeignKey(
                        name: "FK_JobResumes_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "JobApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppStatusLog_JobApplicationId",
                table: "AppStatusLog",
                column: "JobApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_JobResumes_JobApplicationId",
                table: "JobResumes",
                column: "JobApplicationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppStatusLog");

            migrationBuilder.DropTable(
                name: "JobResumes");

            migrationBuilder.DropTable(
                name: "UserResumeCreate");

            migrationBuilder.DropTable(
                name: "UserResumeEmail");

            migrationBuilder.DropTable(
                name: "JobApplications");
        }
    }
}
