package net.atos.daf.postgre.connection;

import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.postgre.util.DafConstants;

public class PostgreConnection implements Serializable{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(PostgreConnection.class);
	private static PostgreConnection instance;
	
	
	
	public static synchronized  PostgreConnection getInstance() {
		if (instance == null) {
				instance = new PostgreConnection();
				
			}
		return instance;
	}
	
	public Connection getConnection(String serverNm, int port, String databaseNm, String userNm,
			String password, String driverNm)throws Exception{
		
		Class.forName(driverNm);
		return DriverManager.getConnection(createValidUrlToConnectPostgreSql(serverNm, port, databaseNm, userNm, password));
		
	}
	
	private String createValidUrlToConnectPostgreSql(String serverNm, int port, String databaseNm, String userNm,
			String password) throws Exception {

		String encodedPassword = encodeValue(password);
		String url = serverNm + ":" + port + "/" + databaseNm + "?" + "user=" + userNm + "&" + "password="
				+ encodedPassword + DafConstants.POSTGRE_SQL_SSL_MODE;

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
