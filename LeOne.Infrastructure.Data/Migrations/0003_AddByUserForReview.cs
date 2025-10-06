using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeOne.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddByUserForReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                schema: "leone",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CreatedByUserId",
                schema: "leone",
                table: "Reviews",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_CreatedByUserId",
                schema: "leone",
                table: "Reviews",
                column: "CreatedByUserId",
                principalSchema: "leone",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_CreatedByUserId",
                schema: "leone",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CreatedByUserId",
                schema: "leone",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "leone",
                table: "Reviews");
        }
    }
}
