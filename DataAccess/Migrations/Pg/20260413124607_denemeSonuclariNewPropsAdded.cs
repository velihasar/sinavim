using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class denemeSonuclariNewPropsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DenemeSinaviId",
                table: "DenemeSinavSonucus",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SinavBolumId",
                table: "DenemeSinavis",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DenemeSinavSonucus_DenemeSinaviId",
                table: "DenemeSinavSonucus",
                column: "DenemeSinaviId");

            migrationBuilder.CreateIndex(
                name: "IX_DenemeSinavis_SinavBolumId",
                table: "DenemeSinavis",
                column: "SinavBolumId");

            migrationBuilder.AddForeignKey(
                name: "FK_DenemeSinavis_SinavBolums_SinavBolumId",
                table: "DenemeSinavis",
                column: "SinavBolumId",
                principalTable: "SinavBolums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DenemeSinavSonucus_DenemeSinavis_DenemeSinaviId",
                table: "DenemeSinavSonucus",
                column: "DenemeSinaviId",
                principalTable: "DenemeSinavis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DenemeSinavis_SinavBolums_SinavBolumId",
                table: "DenemeSinavis");

            migrationBuilder.DropForeignKey(
                name: "FK_DenemeSinavSonucus_DenemeSinavis_DenemeSinaviId",
                table: "DenemeSinavSonucus");

            migrationBuilder.DropIndex(
                name: "IX_DenemeSinavSonucus_DenemeSinaviId",
                table: "DenemeSinavSonucus");

            migrationBuilder.DropIndex(
                name: "IX_DenemeSinavis_SinavBolumId",
                table: "DenemeSinavis");

            migrationBuilder.DropColumn(
                name: "DenemeSinaviId",
                table: "DenemeSinavSonucus");

            migrationBuilder.DropColumn(
                name: "SinavBolumId",
                table: "DenemeSinavis");
        }
    }
}
