using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class DenemeHedefi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KısaAd",
                table: "Sinavs",
                newName: "KisaAd");

            migrationBuilder.CreateTable(
                name: "KullaniciDersNetHedefis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DersId = table.Column<int>(type: "integer", nullable: false),
                    SinavBolumId = table.Column<int>(type: "integer", nullable: true),
                    HedefNet = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDersNetHedefis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciDersNetHedefis_Derses_DersId",
                        column: x => x.DersId,
                        principalTable: "Derses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciDersNetHedefis_SinavBolums_SinavBolumId",
                        column: x => x.SinavBolumId,
                        principalTable: "SinavBolums",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KullaniciDersNetHedefis_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersNetHedefis_DersId",
                table: "KullaniciDersNetHedefis",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersNetHedefis_SinavBolumId",
                table: "KullaniciDersNetHedefis",
                column: "SinavBolumId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDersNetHedefis_UserId_DersId",
                table: "KullaniciDersNetHedefis",
                columns: new[] { "UserId", "DersId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KullaniciDersNetHedefis");

            migrationBuilder.RenameColumn(
                name: "KisaAd",
                table: "Sinavs",
                newName: "KısaAd");
        }
    }
}
