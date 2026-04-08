using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class SinavBolumaddedBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "SinavBolums",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SinavBolums",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SinavBolums",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "SinavBolums",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "SinavBolums",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SinavBolums");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SinavBolums");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SinavBolums");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SinavBolums");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "SinavBolums");
        }
    }
}
