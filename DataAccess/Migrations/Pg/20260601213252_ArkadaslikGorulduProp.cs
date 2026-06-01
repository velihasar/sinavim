using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class ArkadaslikGorulduProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GonderenKabulGordu",
                table: "ArkadaslikIstekleri",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArkadaslikIstekleri_GonderenUserId_Durum_GonderenKabulGordu",
                table: "ArkadaslikIstekleri",
                columns: new[] { "GonderenUserId", "Durum", "GonderenKabulGordu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArkadaslikIstekleri_GonderenUserId_Durum_GonderenKabulGordu",
                table: "ArkadaslikIstekleri");

            migrationBuilder.DropColumn(
                name: "GonderenKabulGordu",
                table: "ArkadaslikIstekleri");
        }
    }
}
