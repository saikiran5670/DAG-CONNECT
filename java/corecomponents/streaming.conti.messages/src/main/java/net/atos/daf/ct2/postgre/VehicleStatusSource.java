package net.atos.daf.ct2.postgre;

import net.atos.daf.ct2.models.scheamas.VehicleStatusSchema;
import net.atos.daf.postgre.connection.PostgreConnection;
import org.apache.flink.streaming.api.functions.source.RichSourceFunction;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.Objects;
import java.util.Properties;

import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_CDC_FETCH_DATA_QUERY;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DATABASE_NAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_DRIVER;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_HOSTNAME;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PASSWORD;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_PORT;
import static net.atos.daf.ct2.constant.DAFCT2Constant.POSTGRE_USER;

public class VehicleStatusSource extends RichSourceFunction<VehicleStatusSchema> implements Serializable {
    private static final Logger logger = LoggerFactory.getLogger(VehicleStatusSource.class);
    private volatile boolean isCancelled=true;
    private Properties properties;
    private Connection masterConnection=null;
    private  ResultSet resultSet;

    public VehicleStatusSource(Properties properties){
        this.properties = properties;
    }

    @Override
    public void run(SourceContext<VehicleStatusSchema> ctx) throws Exception {
         try{
             while (isCancelled){
                 if(Objects.nonNull(resultSet) && resultSet.next()){
                     VehicleStatusSchema vehicleStatusSchema = VehicleStatusSchema.builder()
                             .vin(String.valueOf(resultSet.getObject(1)))
                             .vid(String.valueOf(resultSet.getObject(2)))
                             .status(String.valueOf(resultSet.getObject(3)))
                             .fuelType(String.valueOf(resultSet.getObject(4)))
                             .build();
                     ctx.collect(vehicleStatusSchema);
                 }
             }
         }catch (Exception ex){
             logger.error("Error while reading data from result set conti");
             logger.error("{}",ex);
         }
    }
    @Override
    public void open(org.apache.flink.configuration.Configuration parameters) throws Exception {
        try{
            String serverNm= new String("jdbc:postgresql://")+properties.getProperty(POSTGRE_HOSTNAME);
            int port = Integer.valueOf(properties.getProperty(POSTGRE_PORT));
            String databaseNm = properties.getProperty(POSTGRE_DATABASE_NAME);
            String userNm = properties.getProperty(POSTGRE_USER);
            String password = properties.getProperty(POSTGRE_PASSWORD);
            String driverNm = properties.getProperty(POSTGRE_DRIVER);
            masterConnection=PostgreConnection.getInstance()
                    .getConnection(serverNm, port, databaseNm, userNm, password, driverNm);
            logger.info("Postgre connection established {}", masterConnection.getClientInfo());
            PreparedStatement preparedStatement = masterConnection.prepareStatement(properties.getProperty(POSTGRE_CDC_FETCH_DATA_QUERY));
            resultSet = preparedStatement.executeQuery();
        }catch (Exception ex){
            logger.error("Error while establishing connection from conti job to vehicle status");
            logger.error("{}"+ex);
        }

    }

    @Override
    public void cancel() {
        isCancelled = false;
    }
}
