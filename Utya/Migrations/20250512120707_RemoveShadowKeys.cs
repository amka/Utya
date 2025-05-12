using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utya.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId1",
                table: "ShortLinks");

            migrationBuilder.DropIndex(
                name: "IX_ShortLinks_UserId1",
                table: "ShortLinks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ShortLinks");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ShortLinks",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortLinks_UserId",
                table: "ShortLinks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId",
                table: "ShortLinks");

            migrationBuilder.DropIndex(
                name: "IX_ShortLinks_UserId",
                table: "ShortLinks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ShortLinks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "ShortLinks",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShortLinks_UserId1",
                table: "ShortLinks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortLinks_AspNetUsers_UserId1",
                table: "ShortLinks",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
