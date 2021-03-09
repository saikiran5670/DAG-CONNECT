package net.atos.daf.common.ct2.postgre;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.SQLException;

import org.postgresql.jdbc3.Jdbc3PoolingDataSource;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.common.ct2.util.DAFConstants;

public class PostgreDataSourceConnectionBackup {

	private static Jdbc3PoolingDataSource dSource;
	private static Connection conn;

	public static Jdbc3PoolingDataSource getDataSource(String serverNm, int port, String databaseNm, String userNm,
			String password) {
		try {
			String url = createValidUrlToConnectPostgreSql(serverNm, port, databaseNm, userNm, password);
			dSource = new Jdbc3PoolingDataSource();
			dSource.setUrl(url);
			System.out.println("URL submitted to DataSource  " + url);
			dSource.setMaxConnections(Integer.valueOf(30));
		} catch (TechnicalException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return dSource;

	}

	public static Connection getDataSourceConnection(Jdbc3PoolingDataSource dataSource) {

		try {
			if (null == conn)
				conn = dataSource.getConnection();
			
			System.out.println("Connection is created" + conn);
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return conn;
	}

	private static String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws TechnicalException {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DAFConstants.POSTGRE_SQL_SSL_MODE;

		System.out.println("url = " + url);

		return url;
	}

	private static String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}
}
