using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class SinavBolumAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SinavBolumId",
                table: "Derses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SinavBolums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SinavId = table.Column<int>(type: "integer", nullable: false),
                    Isim = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavBolums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SinavBolums_Sinavs_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Derses_SinavBolumId",
                table: "Derses",
                column: "SinavBolumId");

            migrationBuilder.CreateIndex(
                name: "IX_SinavBolums_SinavId",
                table: "SinavBolums",
                column: "SinavId");

            migrationBuilder.AddForeignKey(
                name: "FK_Derses_SinavBolums_SinavBolumId",
                table: "Derses",
                column: "SinavBolumId",
                principalTable: "SinavBolums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Derses_SinavBolums_SinavBolumId",
                table: "Derses");

            migrationBuilder.DropTable(
                name: "SinavBolums");

            migrationBuilder.DropIndex(
                name: "IX_Derses_SinavBolumId",
                table: "Derses");

            migrationBuilder.DropColumn(
                name: "SinavBolumId",
                table: "Derses");
        }
    }
}
