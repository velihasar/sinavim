using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class KullaniciKonuIlerlemeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DenemeSinavis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SinavId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenemeSinavis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenemeSinavis_Sinavs_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DenemeSinavis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Derses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: true),
                    SinavId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Derses_Sinavs_SinavId",
                        column: x => x.SinavId,
                        principalTable: "Sinavs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DenemeSinavSonucus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DersId = table.Column<int>(type: "integer", nullable: false),
                    DogruSayisi = table.Column<int>(type: "integer", nullable: false),
                    YanlisSayisi = table.Column<int>(type: "integer", nullable: false),
                    BosSayisi = table.Column<int>(type: "integer", nullable: false),
                    ToplamNet = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenemeSinavSonucus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenemeSinavSonucus_Derses_DersId",
                        column: x => x.DersId,
                        principalTable: "Derses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Konus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "text", nullable: true),
                    SiraNo = table.Column<int>(type: "integer", nullable: false),
                    DersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Konus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Konus_Derses_DersId",
                        column: x => x.DersId,
                        principalTable: "Derses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DenemeSinavis_SinavId",
                table: "DenemeSinavis",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_DenemeSinavis_UserId",
                table: "DenemeSinavis",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DenemeSinavSonucus_DersId",
                table: "DenemeSinavSonucus",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Derses_SinavId",
                table: "Derses",
                column: "SinavId");

            migrationBuilder.CreateIndex(
                name: "IX_Konus_DersId",
                table: "Konus",
                column: "DersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DenemeSinavis");

            migrationBuilder.DropTable(
                name: "DenemeSinavSonucus");

            migrationBuilder.DropTable(
                name: "Konus");

            migrationBuilder.DropTable(
                name: "Derses");
        }
    }
}
