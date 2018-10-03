using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class Events : Migration
    {
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: _tablePrefix + "UnpublishedEvent",
                schema: _schema);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscribeAsOf",
                schema: _schema,
                table: _tablePrefix + "Subscription",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: _tablePrefix + "Event",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EventData = table.Column<string>(nullable: true),
                    EventId = table.Column<Guid>(nullable: false),
                    EventKey = table.Column<string>(maxLength: 200, nullable: true),
                    EventName = table.Column<string>(maxLength: 200, nullable: true),
                    EventTime = table.Column<DateTime>(nullable: false),
                    IsProcessed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.PersistenceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventId",
                schema: _schema,
                table: _tablePrefix + "Event",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventTime",
                schema: _schema,
                table: _tablePrefix + "Event",
                column: "EventTime");

            migrationBuilder.CreateIndex(
                name: "IX_Event_IsProcessed",
                schema: _schema,
                table: _tablePrefix + "Event",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventName_EventKey",
                schema: _schema,
                table: _tablePrefix + "Event",
                columns: new[] { "EventName", "EventKey" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: _tablePrefix + "Event",
                schema: _schema);

            migrationBuilder.DropColumn(
                name: "SubscribeAsOf",
                schema: _schema,
                table: _tablePrefix + "Subscription");

            migrationBuilder.CreateTable(
                name: _tablePrefix + "UnpublishedEvent",
                schema: _schema,
                columns: table => new
                {
                    PersistenceId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EventData = table.Column<string>(nullable: true),
                    EventKey = table.Column<string>(maxLength: 200, nullable: true),
                    EventName = table.Column<string>(maxLength: 200, nullable: true),
                    PublicationId = table.Column<Guid>(nullable: false),
                    StepId = table.Column<int>(nullable: false),
                    WorkflowId = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnpublishedEvent", x => x.PersistenceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnpublishedEvent_PublicationId",
                schema: _schema,
                table: _tablePrefix + "UnpublishedEvent",
                column: "PublicationId",
                unique: true);
        }
    }
}
