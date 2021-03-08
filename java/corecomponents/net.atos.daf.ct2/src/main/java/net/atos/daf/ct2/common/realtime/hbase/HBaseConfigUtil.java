package net.atos.daf.ct2.common.realtime.hbase;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Connection;
import org.apache.hadoop.hbase.client.ConnectionFactory;
import org.apache.hadoop.hbase.client.Table;

import net.atos.daf.ct2.common.util.DafConstants;

public class HBaseConfigUtil {
		
	private static Connection connection = null;
	private  static Configuration conf = null;
   
    public static Table getTable(Connection conn, String tableName) throws Exception {
		Table table = null;
		try {

			if (conn != null) {

				table = conn.getTable(TableName.valueOf(tableName));

			}

		} catch (final Exception e) {
			throw new Exception(e.getMessage(), e);

		}
		return table;
	}
    
    public static Connection getHbaseClientConnection(Configuration conf) throws Exception {

		try {
		
			if (null == connection ) 
				connection = ConnectionFactory.createConnection(conf);
			
		} catch (final Exception e) {
			// logger.critical(ErrorCodes.HBASE_CONNECTION_ERROR ," Error while
			// Hbase table connection ==>"+ e.getMessage());
			throw new Exception(e.getMessage(), e);
		}

		return connection;
	}
    
    public static Configuration createConf(ParameterTool envParams)
    {
		
	  conf = HBaseConfiguration.create();
    	
		conf.set(DafConstants.HBASE_ZOOKEEPER_QUORUM,envParams.get(DafConstants.HBASE_ZOOKEEPER_QUORUM));
		conf.set(DafConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT,envParams.get(DafConstants.HBASE_ZOOKEEPER_PROPERTY_CLIENTPORT));
		conf.set(DafConstants.ZOOKEEPER_ZNODE_PARENT,envParams.get(DafConstants.ZOOKEEPER_ZNODE_PARENT)); 
		conf.set(DafConstants.HBASE_REGIONSERVER,envParams.get(DafConstants.HBASE_REGIONSERVER));
		conf.set(DafConstants.HBASE_MASTER,envParams.get(DafConstants.HBASE_MASTER));
		conf.set(DafConstants.HBASE_REGIONSERVER_PORT,envParams.get(DafConstants.HBASE_REGIONSERVER_PORT));
		// conf.set(DafConstants.HBASE_ROOTDIR,envParams.get(DafConstants.HBASE_ROOTDIR));
		
		return conf;
		
	}
	
	
}
