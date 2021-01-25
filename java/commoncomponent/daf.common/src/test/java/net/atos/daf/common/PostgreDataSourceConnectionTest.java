package net.atos.daf.common;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import java.io.FileInputStream;
import java.io.IOException;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Properties;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;
import org.postgresql.jdbc3.Jdbc3PoolingDataSource;
//import org.apache.flink.api.java.utils.ParameterTool;

import net.atos.daf.common.ct2.postgre.PostgreDataSourceConnection;

@RunWith(JUnit4.class)
public class PostgreDataSourceConnectionTest {

	// @Rule
	private PostgreDataSourceConnection dataSource;
	private Connection conn;

	@Test
	public void getDataSourceValue() {
		try (FileInputStream input = new FileInputStream("src/main/resources/env.properties")) {

			Properties envParams = new Properties();
			envParams.load(input);
			
			// Jdbc3PoolingDataSource source =
			// PostgreDataSourceConnection.getDataSource(
			// "jdbc:postgresql://dafct-dev0-dta-cdp-pgsql.postgres.database.azure.com",
			// 5432, "vehicledatamart",
			// "pgadmin@dafct-dev0-dta-cdp-pgsql", "W%PQ1AI}Y97");

			Jdbc3PoolingDataSource source = PostgreDataSourceConnection.getDataSource(
					envParams.getProperty("server_name"), Integer.valueOf(envParams.getProperty("port")),
					envParams.getProperty("database_name"), envParams.getProperty("userId"),
					envParams.getProperty("postgresql_password"));
			String url = source.getUrl();
			System.out.println(url);
			conn = PostgreDataSourceConnection.getDataSourceConnection(source);
			assertNotNull(conn);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} finally {
			try {
				conn.close();
			} catch (SQLException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
}
