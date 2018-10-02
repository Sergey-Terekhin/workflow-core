using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using WorkflowCore.Persistence.EntityFramework;

namespace WorkflowCore.Persistence.PostgreSQL.Migrations
{
    public partial class PersistOutcome : Migration
    {
        private readonly string _tablePrefix = MigrationMetaInfo.TableNamePrefix;
        private readonly string _schema = MigrationMetaInfo.DbSchema;
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
