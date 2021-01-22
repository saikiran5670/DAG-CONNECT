package net.atos.daf.ct2.kafka;

import net.atos.daf.ct2.constant.DAFCT2Constant;

import java.sql.*;
import java.util.Properties;

public class SqlConnection {

  private final Properties properties;
  private Connection connection;
  private Statement statement;

  public SqlConnection(Properties properties) {
    this.properties = properties;
    connection();
  }

  private void connection() {

    try {

      Class.forName(this.properties.getProperty(DAFCT2Constant.POSTGRE_DRIVER));
      this.connection =
          DriverManager.getConnection(
              "jdbc:postgresql://"
                  + this.properties.getProperty(DAFCT2Constant.POSTGRE_HOSTNAME)
                  + ":"
                  + this.properties.getProperty(DAFCT2Constant.POSTGRE_PORT)
                  + "/"
                  + this.properties.getProperty(DAFCT2Constant.POSTGRE_DATABASE_NAME),
              this.properties.getProperty(DAFCT2Constant.POSTGRE_USER),
              this.properties.getProperty(DAFCT2Constant.POSTGRE_PASSWORD));
      this.statement = this.connection.createStatement();

    } catch (SQLException | ClassNotFoundException e) {
      e.printStackTrace();
    }
  }

  public void creation(String createSql) {
    try {
      this.statement.executeUpdate(createSql);

    } catch (SQLException e) {
      e.printStackTrace();
    }
  }

  public void insertion(String insertSql, String vid, String vin) {

    PreparedStatement preparedStatement = null;

    try {
      preparedStatement =
          this.connection.prepareStatement(insertSql, Statement.RETURN_GENERATED_KEYS);

      preparedStatement.setString(1, vid);
      preparedStatement.setString(2, vin);
      preparedStatement.executeUpdate();
      preparedStatement.close();

    } catch (SQLException e) {
      e.printStackTrace();
    }
  }

  public void deletion(String dropSql) {
    try {
      this.statement.executeUpdate(dropSql);

    } catch (SQLException e) {
      e.printStackTrace();
    }
  }

  public void cleanup() {
    try {
      this.statement.close();
      this.connection.close();

    } catch (SQLException e) {
      e.printStackTrace();
    }
  }
}
