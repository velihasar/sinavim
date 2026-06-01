using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.Migrations.Pg
{
    /// <inheritdoc />
    public partial class Arkadaslik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArkadaslikIstekleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GonderenUserId = table.Column<int>(type: "integer", nullable: false),
                    HedefUserId = table.Column<int>(type: "integer", nullable: false),
                    Durum = table.Column<int>(type: "integer", nullable: false),
                    KullanilanDavetKodu = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    YanitTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArkadaslikIstekleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArkadaslikIstekleri_Users_GonderenUserId",
                        column: x => x.GonderenUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArkadaslikIstekleri_Users_HedefUserId",
                        column: x => x.HedefUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciDavetKodlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Kod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciDavetKodlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciDavetKodlari_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Arkadasliklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserIdKucuk = table.Column<int>(type: "integer", nullable: false),
                    UserIdBuyuk = table.Column<int>(type: "integer", nullable: false),
                    ArkadaslikIstegiId = table.Column<int>(type: "integer", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arkadasliklar", x => x.Id);
                    table.CheckConstraint("CK_Arkadaslik_UserIdSiralama", "\"UserIdKucuk\" < \"UserIdBuyuk\"");
                    table.ForeignKey(
                        name: "FK_Arkadasliklar_ArkadaslikIstekleri_ArkadaslikIstegiId",
                        column: x => x.ArkadaslikIstegiId,
                        principalTable: "ArkadaslikIstekleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Arkadasliklar_Users_UserIdBuyuk",
                        column: x => x.UserIdBuyuk,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Arkadasliklar_Users_UserIdKucuk",
                        column: x => x.UserIdKucuk,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArkadaslikIstekleri_GonderenUserId",
                table: "ArkadaslikIstekleri",
                column: "GonderenUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArkadaslikIstekleri_GonderenUserId_HedefUserId_Durum",
                table: "ArkadaslikIstekleri",
                columns: new[] { "GonderenUserId", "HedefUserId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_ArkadaslikIstekleri_HedefUserId",
                table: "ArkadaslikIstekleri",
                column: "HedefUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Arkadasliklar_ArkadaslikIstegiId",
                table: "Arkadasliklar",
                column: "ArkadaslikIstegiId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arkadasliklar_UserIdBuyuk",
                table: "Arkadasliklar",
                column: "UserIdBuyuk");

            migrationBuilder.CreateIndex(
                name: "IX_Arkadasliklar_UserIdKucuk",
                table: "Arkadasliklar",
                column: "UserIdKucuk");

            migrationBuilder.CreateIndex(
                name: "IX_Arkadasliklar_UserIdKucuk_UserIdBuyuk",
                table: "Arkadasliklar",
                columns: new[] { "UserIdKucuk", "UserIdBuyuk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDavetKodlari_Kod",
                table: "KullaniciDavetKodlari",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciDavetKodlari_UserId",
                table: "KullaniciDavetKodlari",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arkadasliklar");

            migrationBuilder.DropTable(
                name: "KullaniciDavetKodlari");

            migrationBuilder.DropTable(
                name: "ArkadaslikIstekleri");
        }
    }
}
