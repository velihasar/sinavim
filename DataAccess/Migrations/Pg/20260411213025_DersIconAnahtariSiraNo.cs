using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class DersIconAnahtariSiraNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Index",
                table: "Sinavs",
                newName: "SiraNo");

            migrationBuilder.AddColumn<string>(
                name: "IkonAnahtari",
                table: "Derses",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiraNo",
                table: "Derses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IkonAnahtari",
                table: "Derses");

            migrationBuilder.DropColumn(
                name: "SiraNo",
                table: "Derses");

            migrationBuilder.RenameColumn(
                name: "SiraNo",
                table: "Sinavs",
                newName: "Index");
        }
    }
}
