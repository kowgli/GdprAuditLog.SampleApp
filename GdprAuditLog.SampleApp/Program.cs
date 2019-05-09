using System.Data;

namespace GdprAuditLog.SampleApp
{
    internal class Program
    {
        private static void Main()
        {
            // ***********
            // Setup
            // ***********
            string loggingDbConnString = @"Data Source=.\SQL2017;Integrated Security=true;Initial Catalog=GdprAuditLog;Persist Security Info=True;";

            var logger = new SqlDataAccessLogger.Logger(new Models.Configuration.AuditConfiguration[]
                    { // This is a single configuration, there can be more than 1
                        new Models.Configuration.AuditConfiguration
                        {
                            BlacklistUsers = new Models.Configuration.BlacklistUser[]{ },
                            EntityFilters = new Models.Configuration.EntityFilter[]
                            {
                                new Models.Configuration.EntityFilter // Entity == table when we look at this from an SQL point of view
                                {
                                    EntitySchemaName = "Table1",
                                    FieldFilters = new Models.Configuration.FieldFilter[]
                                    {
                                        new Models.Configuration.FieldFilter
                                        {
                                            FieldSchemaName = "Field1",
                                            NotLogged = false,
                                            NotTriggering = false
                                        },
                                        new Models.Configuration.FieldFilter
                                        {
                                            FieldSchemaName = "Field2",
                                            NotLogged = false,
                                            NotTriggering = false
                                        }
                                    },
                                    FieldValueFilters = new Models.Configuration.FieldValueFilter[]
                                    {
                                        new Models.Configuration.FieldValueFilter // Will only log if Field1 == "abc123"
                                        {
                                            FieldSchemaName = "Field1",
                                            Operator = Models.Configuration.FieldValueFilter.Operators.Equal,
                                            Value = "abc123"
                                        }
                                    },
                                    LogOnlyPrimaryKey = false,
                                    PrimaryKeySchemaName = "Table1Id"
                                }
                            },
                            Name = "Test GDPR log", // Identifies the logger
                            SystemId = "TestSystem", // Put in unique system name, will be stored in the log table
                            Targets = new Models.Configuration.Target[]
                            {
                                new Models.Configuration.Target
                                {
                                    Order = 1,
                                    OnlyForFallback = false,
                                    PersistorQualifiedName = "SqlXmlPersistor.Persistor",
                                    ConnectionString = loggingDbConnString
                                }
                            }
                        }
                    });


            // TIP!
            // Store the EntityFilters[] as JSON / XML in a configuration file and deserialize before use,
            // so that it can be changed without recompiling the application

            // ***********
            // Logging
            // ***********

            // Create some sample data
            DataTable dt1 = new DataTable("Table1");
            dt1.Columns.Add("Table1Id", typeof(int));
            dt1.Columns.Add("Field1", typeof(string));
            dt1.Columns.Add("Field2", typeof(int));

            dt1.Rows.Add(new object[] { 1, "test", 100 }); // Will not be logged, due to field value filter
            dt1.Rows.Add(new object[] { 2, "abc123", 200 });
            dt1.Rows.Add(new object[] { 3, "abc123", 300 });

            DataTable dt2 = new DataTable("Table2"); // Will not be logged, because not in entity filter
            dt2.Columns.Add("Table2Id", typeof(int));

            dt2.Rows.Add(new object[] { 1 });
            dt2.Rows.Add(new object[] { 2 });

            // Actual logging
            logger.Audit(dt1, "user1", "SELECT * FROM Table1");
            logger.Audit(dt2, "user1", "SELECT * FROM Table2");

            // ************
            // State of DB after loggin
            // ************

            /*
             * This record was created:
             *
             * AuditEntryId: D9C1FDB2-5144-4023-B1DE-51BEE773A719
             * DataRetrievedOnUtc: 2019-05-09 08:14:24.157
             * UserLogin: user1
             * UserFullName: user1
             * EntityName: Table1
             * Type: SQL
             * SystemId: TestSystem
             * RowCount: 2
             * Query: <Query xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><SqlQuery>SELECT * FROM Table1</SqlQuery></Query>
             * Data: <AuditEntry xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Records><Record><EntityFields><EntityFields><EntityName>Table1</EntityName><Fields><Field><Name>Field1</Name><Value>abc123</Value></Field><Field><Name>Field2</Name><Value>200</Value></Field></Fields></EntityFields></EntityFields></Record><Record><EntityFields><EntityFields><EntityName>Table1</EntityName><Fields><Field><Name>Field1</Name><Value>abc123</Value></Field><Field><Name>Field2</Name><Value>300</Value></Field></Fields></EntityFields></EntityFields></Record></Records></AuditEntry>
             */
        }
    }
}