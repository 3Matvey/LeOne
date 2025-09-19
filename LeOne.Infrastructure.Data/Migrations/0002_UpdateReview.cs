using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeOne.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ByUserId",
                schema: "leone",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                schema: "leone",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                schema: "leone",
                table: "Reviews",
                newName: "EntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntityId",
                schema: "leone",
                table: "Reviews",
                newName: "TransactionId");

            migrationBuilder.AddColumn<Guid>(
                name: "ByUserId",
                schema: "leone",
                table: "Reviews",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                schema: "leone",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
