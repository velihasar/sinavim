using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class DigerEntitylerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KullaniciKonuIlerlemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    KonuId = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciKonuIlerlemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciKonuIlerlemes_Konus_KonuId",
                        column: x => x.KonuId,
                        principalTable: "Konus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciKonuIlerlemes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciSinavs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SinavId = table.Column<int>(type: "integer", nullable: false),
                    HedefPuan = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciSinavs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciSinavs_Sinavs_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciSinavs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKonuIlerlemes_KonuId",
                table: "KullaniciKonuIlerlemes",
                column: "KonuId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKonuIlerlemes_UserId",
                table: "KullaniciKonuIlerlemes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSinavs_SinavId",
                table: "KullaniciSinavs",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciSinavs_UserId",
                table: "KullaniciSinavs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciKonuIlerlemes");

            migrationBuilder.DropTable(
                name: "KullaniciSinavs");
        }
    }
}
