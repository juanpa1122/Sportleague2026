using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SportsLeague.DataAccess.Context;

#nullable disable

namespace SportsLeague.DataAccess.Migrations
{
    [DbContext(typeof(LeagueDbContext))]
    [Migration("20260512000200_AddMatchResult_Goal_Card")]
    public partial class AddMatchResult_Goal_Card : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    HomeGoals = table.Column<int>(type: "int", nullable: false),
                    AwayGoals = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.Id);
                    table.ForeignKey("FK_MatchResults_Matches_MatchId", x => x.MatchId, "Matches", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey("FK_Cards_Matches_MatchId", x => x.MatchId, "Matches", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Cards_Players_PlayerId", x => x.PlayerId, "Players", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey("FK_Goals_Matches_MatchId", x => x.MatchId, "Matches", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Goals_Players_PlayerId", x => x.PlayerId, "Players", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(name: "IX_Cards_MatchId", table: "Cards", column: "MatchId");
            migrationBuilder.CreateIndex(name: "IX_Cards_PlayerId", table: "Cards", column: "PlayerId");
            migrationBuilder.CreateIndex(name: "IX_Goals_MatchId", table: "Goals", column: "MatchId");
            migrationBuilder.CreateIndex(name: "IX_Goals_PlayerId", table: "Goals", column: "PlayerId");
            migrationBuilder.CreateIndex(name: "IX_MatchResults_MatchId", table: "MatchResults", column: "MatchId", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Cards");
            migrationBuilder.DropTable(name: "Goals");
            migrationBuilder.DropTable(name: "MatchResults");
        }
    }
}
