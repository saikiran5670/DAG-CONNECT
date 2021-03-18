package net.atos.daf.postgre.connection;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.SQLException;

import org.postgresql.jdbc3.Jdbc3PoolingDataSource;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.common.ct2.exception.TechnicalException;
import net.atos.daf.postgre.util.DafConstants;

public class PostgreDataSourceConnection {

	private static PostgreDataSourceConnection instance;
	private Jdbc3PoolingDataSource dSource;
	private static Logger logger = LoggerFactory.getLogger(PostgreDataSourceConnection.class);

	private PostgreDataSourceConnection() {

	}

	public static PostgreDataSourceConnection getInstance() {
		if (instance == null) {
			synchronized (PostgreDataSourceConnection.class) {
				if (instance == null) {
					// if instance is null, initialize
					instance = new PostgreDataSourceConnection();
				}
			}
		}
		return instance;
	}

	public Connection getDataSourceConnection(String serverNm, int port, String databaseNm, String userNm,
			String password) {
		Connection conn = null;

		try {
			String url = createValidUrlToConnectPostgreSql(serverNm, port, databaseNm, userNm, password);
			dSource = new Jdbc3PoolingDataSource();
			dSource.setUrl(url);
			System.out.println("URL submitted to DataSource  " + url);
			dSource.setMaxConnections(DafConstants.MAX_CONNECTIONS);
			conn = dSource.getConnection();
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (TechnicalException e) {
			e.printStackTrace();

			while (DafConstants.RETRY_COUNTER < DafConstants.MAX_RETRIES) {
				try {
					if (null == conn) {
						conn = dSource.getConnection();
						return conn;
					}
					System.out.println("Connection is created" + conn);
				} catch (SQLException e1) {
					DafConstants.RETRY_COUNTER++;
					logger.error("FAILED - Command failed on retry " + DafConstants.RETRY_COUNTER + " of "
							+ DafConstants.MAX_RETRIES, e1);
					if (DafConstants.RETRY_COUNTER >= DafConstants.MAX_RETRIES) {
						logger.error("Max retries exceeded.");
						break;
					}

				}

			}
		}
		return conn;
	}

	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws TechnicalException {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;
		System.out.println("url = " + url);

		return url;
	}

	private String encodeValue(String value) {
		try {
			return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
		} catch (UnsupportedEncodingException ex) {
			throw new RuntimeException(ex.getCause());
		}
	}
}
