using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Coiner.Business.Migrations
{
    public partial class AddNotificationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationConfiguration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NotificationOutputType = table.Column<int>(nullable: false),
                    NotificationUpdateFrequency = table.Column<int>(nullable: false),
                    SendDay = table.Column<int>(nullable: false),
                    SendTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    NotificationCategory = table.Column<int>(nullable: false),
                    NotificationConfigurationId = table.Column<int>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplate_NotificationConfiguration_NotificationConfigurationId",
                        column: x => x.NotificationConfigurationId,
                        principalTable: "NotificationConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationProduced",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AppReadStatus = table.Column<bool>(nullable: false),
                    AppReadTime = table.Column<DateTimeOffset>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(nullable: false),
                    NotificationOutputType = table.Column<int>(nullable: false),
                    NotificationTemplateId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationProduced", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationProduced_NotificationTemplate_NotificationTemplateId",
                        column: x => x.NotificationTemplateId,
                        principalTable: "NotificationTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NotificationProducedId = table.Column<int>(nullable: false),
                    SendEmail = table.Column<string>(nullable: true),
                    SendPhone = table.Column<string>(nullable: true),
                    SendStatus = table.Column<bool>(nullable: false),
                    SendTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationSent_NotificationProduced_NotificationProducedId",
                        column: x => x.NotificationProducedId,
                        principalTable: "NotificationProduced",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationProduced_NotificationTemplateId",
                table: "NotificationProduced",
                column: "NotificationTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSent_NotificationProducedId",
                table: "NotificationSent",
                column: "NotificationProducedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplate_NotificationConfigurationId",
                table: "NotificationTemplate",
                column: "NotificationConfigurationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSent");

            migrationBuilder.DropTable(
                name: "NotificationProduced");

            migrationBuilder.DropTable(
                name: "NotificationTemplate");

            migrationBuilder.DropTable(
                name: "NotificationConfiguration");
        }
    }
}
