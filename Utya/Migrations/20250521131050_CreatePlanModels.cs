using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Utya.Migrations
{
    /// <inheritdoc />
    public partial class CreatePlanModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clicks_ShortLinks_ShortLinkId",
                table: "Clicks");

            migrationBuilder.DropForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShortLinks",
                type: "text",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShortLinkId",
                table: "Clicks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxLinks = table.Column<int>(type: "integer", nullable: false),
                    Expiration = table.Column<int>(type: "integer", nullable: false),
                    MaxClicksPerMonth = table.Column<int>(type: "integer", nullable: false),
                    AllowCustomDomain = table.Column<bool>(type: "boolean", nullable: false),
                    AllowAdvancedAnalytics = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LinksUsed = table.Column<int>(type: "integer", nullable: false),
                    ClicksUsed = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    PlanId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlans_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPlans_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PlanId",
                table: "AspNetUsers",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_ApplicationUserId",
                table: "UserPlans",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_PlanId",
                table: "UserPlans",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Plans_PlanId",
                table: "AspNetUsers",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clicks_ShortLinks_ShortLinkId",
                table: "Clicks",
                column: "ShortLinkId",
                principalTable: "ShortLinks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Plans_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clicks_ShortLinks_ShortLinkId",
                table: "Clicks");

            migrationBuilder.DropForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks");

            migrationBuilder.DropTable(
                name: "UserPlans");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShortLinks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShortLinkId",
                table: "Clicks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tier",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Clicks_ShortLinks_ShortLinkId",
                table: "Clicks",
                column: "ShortLinkId",
                principalTable: "ShortLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
