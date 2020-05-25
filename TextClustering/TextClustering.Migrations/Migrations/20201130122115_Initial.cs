using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TextClustering.Migrations.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(maxLength: 256, nullable: false),
                    LastName = table.Column<string>(maxLength: 256, nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    Password = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: false),
                    CreatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Datasets_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetClusterings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetClusterings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetClusterings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatasetClusterings_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatasetTexts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatasetId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 256, nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatasetTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatasetTexts_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clusters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatasetClusteringId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clusters_DatasetClusterings_DatasetClusteringId",
                        column: x => x.DatasetClusteringId,
                        principalTable: "DatasetClusterings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClusterDatasetTexts",
                columns: table => new
                {
                    DatasetTextId = table.Column<int>(nullable: false),
                    ClusterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterDatasetTexts", x => new { x.ClusterId, x.DatasetTextId });
                    table.ForeignKey(
                        name: "FK_ClusterDatasetTexts_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClusterDatasetTexts_DatasetTexts_DatasetTextId",
                        column: x => x.DatasetTextId,
                        principalTable: "DatasetTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClusterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicTokens_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClusterDatasetTexts_DatasetTextId",
                table: "ClusterDatasetTexts",
                column: "DatasetTextId");

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_DatasetClusteringId",
                table: "Clusters",
                column: "DatasetClusteringId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetClusterings_CreatedById",
                table: "DatasetClusterings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetClusterings_DatasetId",
                table: "DatasetClusterings",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_CreatedById",
                table: "Datasets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetTexts_DatasetId",
                table: "DatasetTexts",
                column: "DatasetId");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetTexts_Key_DatasetId",
                table: "DatasetTexts",
                columns: new[] { "Key", "DatasetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ClusterId",
                table: "Topics",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicTokens_TopicId",
                table: "TopicTokens",
                column: "TopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterDatasetTexts");

            migrationBuilder.DropTable(
                name: "TopicTokens");

            migrationBuilder.DropTable(
                name: "DatasetTexts");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Clusters");

            migrationBuilder.DropTable(
                name: "DatasetClusterings");

            migrationBuilder.DropTable(
                name: "Datasets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
