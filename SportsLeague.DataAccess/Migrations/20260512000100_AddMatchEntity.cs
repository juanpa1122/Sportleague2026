using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SportsLeague.DataAccess.Context;

#nullable disable

namespace SportsLeague.DataAccess.Migrations
{
    [DbContext(typeof(LeagueDbContext))]
    [Migration("20260512000100_AddMatchEntity")]
    public partial class AddMatchEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamId = table.Column<int>(type: "int", nullable: false),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    MatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Matchday = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey("FK_Matches_Referees_RefereeId", x => x.RefereeId, "Referees", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Matches_Teams_AwayTeamId", x => x.AwayTeamId, "Teams", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Matches_Teams_HomeTeamId", x => x.HomeTeamId, "Teams", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_Matches_Tournaments_TournamentId", x => x.TournamentId, "Tournaments", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_Matches_AwayTeamId", table: "Matches", column: "AwayTeamId");
            migrationBuilder.CreateIndex(name: "IX_Matches_HomeTeamId", table: "Matches", column: "HomeTeamId");
            migrationBuilder.CreateIndex(name: "IX_Matches_RefereeId", table: "Matches", column: "RefereeId");
            migrationBuilder.CreateIndex(name: "IX_Matches_TournamentId", table: "Matches", column: "TournamentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Matches");
        }
    }
}
