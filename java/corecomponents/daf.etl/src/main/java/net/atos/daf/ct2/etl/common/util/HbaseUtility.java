package net.atos.daf.ct2.etl.common.util;

import java.io.Serializable;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Connection;
import org.apache.hadoop.hbase.client.ConnectionFactory;
import org.apache.hadoop.hbase.client.HTable;
import org.apache.hadoop.hbase.client.Table;

import net.atos.daf.common.ct2.exception.FailureException;

public class HbaseUtility implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private static Connection connection = null;
	private static Configuration conf = null;

	public static Table getTable(Connection conn, String tableName, String tableNameSpace) throws FailureException {
		Table table = null;
		try {

			if (conn != null) {
				table = conn.getTable(TableName.valueOf(tableNameSpace, tableName));
			}

		} catch (final Exception e) {
			throw new FailureException(e.getMessage(), e);

		}
		return table;
	}

	public static Table getTable(Connection conn, String tableName) throws FailureException {
		Table table = null;
		try {

			if (conn != null) {
				table = conn.getTable(TableName.valueOf(tableName));
			}

		} catch (final Exception e) {
			throw new FailureException(e.getMessage(), e);

		}
		return table;
	}

	public static HTable getHTable(Connection conn, String tableName) throws FailureException {
		HTable table = null;
		try {

			if (conn != null) {
				table = (HTable) conn.getTable(TableName.valueOf(tableName));
			}

		} catch (final Exception e) {
			throw new FailureException(e.getMessage(), e);

		}
		return table;
	}

	public static Connection getHbaseClientConnection(Configuration conf) throws FailureException {

		try {

			if (null == connection)
				connection = ConnectionFactory.createConnection(conf);

		} catch (final Exception e) {
			throw new FailureException(e.getMessage(), e);
		}

		return connection;
	}

	public static Configuration createConf(ParameterTool envParams) {

		conf = HBaseConfiguration.create();

		conf.set(ETLConstants.HBASE_ZOOKEEPER_QUORUM, envParams.get(ETLConstants.HBASE_ZOOKEEPER_QUORUM));
		conf.set(ETLConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT,
				envParams.get(ETLConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT));
		conf.set(ETLConstants.ZOOKEEPER_ZNODE_PARENT, envParams.get(ETLConstants.ZOOKEEPER_ZNODE_PARENT));
		conf.set(ETLConstants.HBASE_MASTER, envParams.get(ETLConstants.HBASE_MASTER));
		conf.set(ETLConstants.HBASE_ROOTDIR, envParams.get(ETLConstants.HBASE_ROOTDIR));

		// TODO Check this parameter
		// conf.set(ETLConstants.HBASE_REGIONSERVER, envParams.get(ETLConstants.HBASE_REGIONSERVER));
		// conf.set(ETLConstants.HBASE_REGIONSERVER_PORT, envParams.get(ETLConstants.HBASE_REGIONSERVER_PORT));

		return conf;

	}

}
