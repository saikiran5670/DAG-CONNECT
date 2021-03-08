package net.atos.daf.hbase.connection;

import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;

import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.TableName;
import org.apache.hadoop.hbase.client.Admin;
import org.apache.hadoop.hbase.client.Get;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Result;
import org.apache.hadoop.hbase.client.ResultScanner;
import org.apache.hadoop.hbase.client.Scan;
import org.apache.hadoop.hbase.client.Table;
import org.apache.hadoop.hbase.util.Bytes;
import org.apache.log4j.LogManager;
import org.apache.log4j.Logger;

public class HbaseAdapter {
    private static final Logger logger = LogManager.getLogger(HbaseAdapter.class);

    private HbaseConnectionPool connectionPool = null;
    
    private static HbaseAdapter hbaseAdapter = null;
   private Configuration configuration=null;
    private HbaseAdapter() { }

    public static HbaseAdapter getInstance() {
        if (null == hbaseAdapter) {
            synchronized (HbaseAdapter.class) {
                if (null == hbaseAdapter) {
                    hbaseAdapter = new HbaseAdapter();
                }
            }
        }
        return  hbaseAdapter;
    }

   
   

	public HbaseConnectionPool getConnection(String quorum,String clientPort,String parent,String regionserver,String master,String port,String tableName) {
       
	try {
     //   	InputStream is = null;
       // 	is = this.getClass().getClassLoader().getResourceAsStream("hbase-conf.properties");
            // load and parse config file
         //  FileInputStream in = new FileInputStream("src/main/resources/hbase-conf.properties");
           // Properties props = new Properties();
           //props.load(in);
            int poolSize = 30;//Integer.parseInt(props.getProperty("hbase.connection.poolsize"));
            int waittime = 500;//Integer.parseInt(props.getProperty("hbase.connection.waittime.millis"));
            int healthCheckInterval =5;// Integer.parseInt(props.getProperty("hbase.connection.health.check.interval.second")); 
            logger.info("hbase connection pool init begin. pool size:" + poolSize
                    + " waitTime:" + waittime + " health check interval:" + healthCheckInterval);

            // create hbase configuration by hase-site.xml
            Configuration conf = HBaseConfiguration.create();
        	conf.set("hbase.zookeeper.quorum",quorum);
    		conf.set("hbase.zookeeper.property.clientPort",clientPort);
    		conf.set("zookeeper.znode.parent",parent); 
    		conf.set("hbase.master",master);
    	/*	conf.set("hbase.regionserver",regionserver);
    		conf.set("hbase.regionserver.port",port);   */      
    		setConfiguration(conf);
            HbaseConfig hbaseConfig = new HbaseConfig( poolSize,
                    waittime, healthCheckInterval, tableName, getConfiguration());

            connectionPool = new HbaseConnectionPool();
            int ret = connectionPool.init(hbaseConfig);
            if (0 != ret) {
                logger.fatal("init connectionPool failed");
               
            }
        } catch (Exception e) {
            e.printStackTrace();
            logger.fatal("Environment not found "+e);
          
        } 
        return connectionPool;
    }

   
    /*
     * Table Exist
     */
    public boolean isExist(String tableName) {
        boolean retValue = false;
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get conn from pool failed");
                return false;
            }
            Admin admin = conn.getConnection().getAdmin();
            TableName tabName = TableName.valueOf(tableName);
            if (admin.tableExists(tabName)) {
                retValue = true;
            }
            admin.close();
        } catch (IOException e) {
            logger.warn("exception:" + e.getMessage());
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return retValue;
    }

   

    /*
     * Get Row by rowkey
     */
    public Result getRowByRowKey(String tableName, String rowkey) {
        Result result = null;
        HbaseConnection conn  = null;
        try {
            TableName tabName = TableName.valueOf(tableName);
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.error("get connection from pool failed");
                return null;
            }
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
            } else {
                Get get = new Get(rowkey.getBytes());
                result = new Result();
                result = table.get(get);
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase get exception" + e.getMessage());
            return result;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return result;
    }

    /*
     *  multiGetRowByRowKey
     */
    public List<Result> multiGetRowByRowKey(String tableName, String[] rowkeys) {
        int rowNum = rowkeys.length;
        if (0 == rowNum) {
            return null;
        }
        List<Result> retList = new ArrayList<Result>();
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get connection from pool failed");
                return null;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                retList = null;
            } else {
                List list = new ArrayList();
                for (int i = 0; i < rowNum; ++i) {
                    Get get = new Get(Bytes.toBytes(rowkeys[i]));
                    list.add(get);
                }
                Result[] resluts = table.get(list);
                for (int i = 0; i < resluts.length; ++i) {
                    retList.add(resluts[i]);
                }
            }
            table.close();
            admin.close();
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase multi get exception" + e.getMessage());
            return null;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return retList;
    }

    /*
     * Put values
     */
    public int putRowByRowKey(String tableName, String rowkey, String columnFamily, String columm, String value) {
        int ret = 0;
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get connection from pool failed");
                return -1;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                ret = -1;
            } else {
                Put put = new Put(Bytes.toBytes(rowkey));
                put.addColumn(Bytes.toBytes(columnFamily), Bytes.toBytes(columm), Bytes.toBytes(value));
                table.put(put);
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase put exception:" + e.getMessage());
            return -1;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return ret;
    }

    /*
     * multiPutRowByRowKey
     */
    public int multiPutRowByRowKey(String tableName, String rowkey,
                                   String columnFamily, String[] columns, String[] values) {

        int colNum = columns.length;
        int valNum = values.length;
        if (colNum != valNum) {
            return -1;
        }
        int ret = 0;
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get connection from pool failed");
                return -1;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                ret = -1;
            } else {
                Put put = new Put(Bytes.toBytes(rowkey));
                for (int i = 0; i < colNum; ++i) {
                    put.addColumn(Bytes.toBytes(columnFamily), Bytes.toBytes(columns[i]), Bytes.toBytes(values[i]));
                }
                table.put(put);
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase multi put exception." + e.getMessage());
            return -1;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return ret;
    }

    /*
     * multiPutRowsByRowKey
     */
    public int multiPutRowsByRowKey(String tableName,
                                    String columnFamily,
                                    final String column,
                                    final String[] rowkeys,
                                    final String[]  valueList) {
        int ret = 0;
        int keySize = rowkeys.length;
        int listSize = valueList.length;
        if (keySize != listSize) {
            logger.warn("rowkeys size not equal to value list size");
            return -1;
        }
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get connection from pool failed");
                return -1;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                ret = -1;
            } else {
                List<Put> putList = new ArrayList<Put>();
                for (int i = 0; i < keySize; ++i) {
                    Put put = new Put(Bytes.toBytes(rowkeys[i]));
                    put.addColumn(Bytes.toBytes(columnFamily), Bytes.toBytes(column), Bytes.toBytes(valueList[i]));
                    putList.add(put);
                }
                table.put(putList);
            }
            table.close();
            admin.close();
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase multiput exception." + e.getMessage());
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return ret;
    }

    public int multiPut(String tableName, List<Put> putList) {
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.warn("get connection from pool failed");
                return -1;
            }
            TableName tabName = TableName.valueOf(tableName);
            Admin admin = conn.getConnection().getAdmin();
            Table table = conn.getConnection().getTable(tabName);
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
            } else {
                table.put(putList);
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
            return -1;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return 0;
    }

    public List<Result> scanRowBy(String tableName) {
        List<Result> list = new ArrayList<Result>();
        ResultScanner results = null;
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.error("get connection from pool failed");
                return null;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                list = null;
            } else {
                Scan scan = new Scan();
                results = table.getScanner(scan);

                for (Result result : results) {
                    list.add(result);
                }
                results.close();
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
            logger.error("hbase scan exception. " + e.getMessage());
            return null;
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return list;
    }

    public List<Result> scanRowByRange(String tableName, String columnFamily, String column,
                                       String beginRow, String endRow) {
        List<Result> list = new ArrayList<Result>();
        ResultScanner results = null;
        HbaseConnection conn = null;
        try {
            conn = connectionPool.getHbaseConnection();
            if (null == conn) {
                logger.error("get connection from pool failed");
                return null;
            }
            TableName tabName = TableName.valueOf(tableName);
            Table table = conn.getConnection().getTable(tabName);
            Admin admin = conn.getConnection().getAdmin();
            if (!admin.isTableEnabled(tabName)) {
                logger.error("table " + tableName + " in hbase is not enable");
                list = null;
            } else {
                Scan scan = new Scan();
                scan.addFamily(Bytes.toBytes(columnFamily));
                scan.setStartRow(Bytes.toBytes(beginRow));
                scan.setStopRow(Bytes.toBytes(endRow + 0)); 
                long begin = System.currentTimeMillis();
                results = table.getScanner(scan);
                long end = System.currentTimeMillis();
                if (null != results) {
                    for (Result result : results) {
                        list.add(result);
                    }
                    results.close();
                } else {
                    logger.warn("scan from hbase result is null");
                }
                logger.info("hbase table getScanner success. result size: " + list.size() + " cost:" + (end - begin) + "ms");
            }
            table.close();
            admin.close();
            table = null;
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            if (conn != null) {
                connectionPool.releaseConnection(conn);
            }
        }
        return list;
    }

	public Configuration getConfiguration() {
		return configuration;
	}

	public void setConfiguration(Configuration configuration) {
		this.configuration = configuration;
	}

	
    
}