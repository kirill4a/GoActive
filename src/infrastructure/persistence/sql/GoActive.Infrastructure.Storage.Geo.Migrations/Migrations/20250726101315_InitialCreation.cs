using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace GoActive.Infrastructure.Storage.Geo.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gin", ",,")
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    external_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    country = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    district = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    settlement = table.Column<string>(type: "character varying(82)", maxLength: 82, nullable: true),
                    street = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    building = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    location = table.Column<Point>(type: "geometry (point)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    normalized_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    altitude = table.Column<float>(type: "real", nullable: true),
                    address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    activities = table.Column<string[]>(type: "text[]", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spots", x => x.id);
                    table.ForeignKey(
                        name: "fk_spots_addresses_address_id",
                        column: x => x.address_id,
                        principalTable: "addresses",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_country",
                table: "addresses",
                column: "country");

            migrationBuilder.CreateIndex(
                name: "ix_addresses_source_external_id",
                table: "addresses",
                columns: new[] { "source", "external_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spots_address_id",
                table: "spots",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_spots_normalized_title_location",
                table: "spots",
                columns: new[] { "normalized_title", "location" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_spots_title",
                table: "spots",
                column: "title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "spots");

            migrationBuilder.DropTable(
                name: "addresses");
        }
    }
}
