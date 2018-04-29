using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Recommend.Api.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectRecommends",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Company = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FinancingStage = table.Column<string>(nullable: true),
                    FromUserAvatar = table.Column<string>(nullable: true),
                    FromUserId = table.Column<int>(nullable: false),
                    FromUserName = table.Column<string>(nullable: true),
                    Introduction = table.Column<string>(nullable: true),
                    ProjectAvatar = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    RecommendType = table.Column<int>(nullable: false),
                    RecommendedTime = table.Column<DateTime>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    UseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRecommends", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectReferenceUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Avatar = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    ProjectRecommendId = table.Column<int>(nullable: true),
                    UseId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectReferenceUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectReferenceUsers_ProjectRecommends_ProjectRecommendId",
                        column: x => x.ProjectRecommendId,
                        principalTable: "ProjectRecommends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReferenceUsers_ProjectRecommendId",
                table: "ProjectReferenceUsers",
                column: "ProjectRecommendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectReferenceUsers");

            migrationBuilder.DropTable(
                name: "ProjectRecommends");
        }
    }
}
