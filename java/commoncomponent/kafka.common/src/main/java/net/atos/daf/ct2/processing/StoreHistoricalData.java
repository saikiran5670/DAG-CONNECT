package net.atos.daf.ct2.processing;

import java.io.IOException;

import org.apache.flink.streaming.api.functions.sink.RichSinkFunction;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Table;
import org.apache.hadoop.hbase.util.Bytes;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import net.atos.daf.ct2.constant.KafkaCT2Constant;
import net.atos.daf.ct2.pojo.KafkaRecord;
import net.atos.daf.hbase.connection.HbaseAdapter;
import net.atos.daf.hbase.connection.HbaseConnection;
import net.atos.daf.hbase.connection.HbaseConnectionPool;

public class StoreHistoricalData extends RichSinkFunction<KafkaRecord<String>> {

	private static final long serialVersionUID = 1L;
	private static final Logger logger = LoggerFactory.getLogger(StoreHistoricalData.class);

	private Table table = null;
	private HbaseConnection conn = null;
	private String zookeeperQuorum = null;
	private String zookeeperClientPort = null;
	private String zookeeperNodeParent = null;
	private String zookeeperRegionServer = null;
	private String hbaseMaster = null;
	private String regionServerPort = null;
	private String tableName = null;
	private String colFm = null;
	
	public StoreHistoricalData(String quorum, String clientPort, String parent, String regionserver, String master,
			String port, String tableNm, String cf) {
		zookeeperQuorum = quorum;
		zookeeperClientPort = clientPort;
		zookeeperNodeParent = parent;
		zookeeperRegionServer = regionserver;
		hbaseMaster = master;
		regionServerPort = port;
		tableName = tableNm;
		colFm = cf;
	}

	@Override
	public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {

		super.open(parameters);

		HbaseAdapter hbaseAdapter = HbaseAdapter.getInstance();
		HbaseConnectionPool connectionPool = hbaseAdapter.getConnection(zookeeperQuorum, zookeeperClientPort,
				zookeeperNodeParent, zookeeperRegionServer, hbaseMaster, regionServerPort, tableName);

		try {
			conn = connectionPool.getHbaseConnection();
			if (null == conn) {
				logger.warn("Getting connection from HBase connection pool failed");
			}
			TableName tblName = TableName.valueOf(tableName);
			table = conn.getConnection().getTable(tblName);

			logger.info("historical tableName " + tableName);

		} catch (IOException e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing HBase connection ::{} " , e.getMessage());
			throw e;
		} catch (Exception e) {
			// TODO: handle exception both logger and throw is not required
			logger.error("Issue while establishing HBase connection ::{} " , e.getMessage());
			throw e;
		}

	}

	public void invoke(KafkaRecord<String> value, Context context) throws Exception {

		//need to check on rowKey
		Put put = new Put(Bytes.toBytes(String.valueOf(value.getKey())));
	
		put.addColumn(Bytes.toBytes(colFm),
				Bytes.toBytes(KafkaCT2Constant.HBASE_HISTORICAL_TABLE_COLNM),
				Bytes.toBytes(String.valueOf(value.getValue())));
		table.put(put);
	}

	@Override
	public void close() throws Exception {

		try {
			if (table != null) {
				table.close();
			}

			if (conn != null) {
				conn.releaseConnection();
			}

		} catch (IOException e) {
			logger.error("Issue while closing historical table HBase connection :: {}", e.getMessage());
		}

	}
}