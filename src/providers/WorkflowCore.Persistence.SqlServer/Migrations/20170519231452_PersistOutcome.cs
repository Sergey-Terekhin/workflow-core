using System;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.SqlServer.Migrations
{
    public partial class PersistOutcome : Migration
    {
        private readonly string _schema = MigrationMetaInfo.DbSchema;
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Outcome",
                schema: _schema,
                table: _tablePrefix + "ExecutionPointer");
        }
    }
}
