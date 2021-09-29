package net.atos.daf.ct2.cache.postgres.impl;


import net.atos.daf.ct2.cache.postgres.RichPostgresMap;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.kafka.AlertCdc;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.configuration.Configuration;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.util.Optional;

import static net.atos.daf.ct2.props.AlertConfigProp.*;

public class RichPostgresMapImpl extends RichPostgresMap<AlertCdc,Payload<Object>> implements Serializable {

    private static final long serialVersionUID = 1L;

    private Connection connection;

    private static final Logger logger = LoggerFactory.getLogger(RichPostgresMapImpl.class);


    public RichPostgresMapImpl(ParameterTool parameterTool){
        super(parameterTool);
    }

    @Override
    public void open(Configuration parameters) throws Exception {
        logger.trace("DB connection open :: {}",parameterTool.getProperties());
        Class.forName(parameterTool.get(DRIVER));

        String connectionUrl = new StringBuilder("jdbc:postgresql://")
                .append(parameterTool.get(MASTER_POSTGRES_HOST))
                .append(":" + parameterTool.get(MASTER_POSTGRES_PORT) + "/")
                .append(parameterTool.get(MASTER_DATABASE))
                .append("?user=" + parameterTool.get(MASTER_USERNAME))
                .append("&password=" + parameterTool.get(MASTER_PASSWORD))
                .append("&sslmode="+parameterTool.get(MASTER_POSTGRES_SSL))
                .toString();

        connection = DriverManager.getConnection(connectionUrl);
    }

    @Override
    public Payload<Object> map(AlertCdc alertCdc) throws Exception {
        ResultSet row = connection.createStatement()
                .executeQuery(parameterTool.get(ALERT_THRESHOLD_FETCH_SINGLE_QUERY) + "" + alertCdc.getAlertId());
        AlertUrgencyLevelRefSchema refSchema=null;
        while (row != null && row.next()){
            refSchema = AlertUrgencyLevelRefSchema.builder()
                    .alertId(Long.valueOf(String.valueOf(row.getObject(1))))
                    .alertCategory(String.valueOf(row.getObject(2)))
                    .alertType(String.valueOf(row.getObject(3)))
                    .alertState(String.valueOf(row.getObject(4)))
                    .urgencyLevelType(String.valueOf(row.getObject(5)))
                    .thresholdValue(row.getObject(6) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(6))))
                    .unitType(String.valueOf(row.getObject(7)))
                    .periodType(String.valueOf(row.getObject(8)))
                    .dayTypeArray(String.valueOf(row.getObject(9)))
                    .startTime(row.getObject(10) == null ? 0L : Long.valueOf(String.valueOf(row.getObject(10))))
                    .endTime(row.getObject(11) == null ? 0L :   Long.valueOf(String.valueOf(row.getObject(11))))
                    .nodeSeq(row.getObject(12) == null ? 0 : Integer.valueOf(String.valueOf(row.getObject(12))))
                    .latitude(row.getObject(13) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(13))))
                    .longitude(row.getObject(14) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(14))))
                    .landmarkId(row.getObject(15) == null? -1 : Integer.valueOf(String.valueOf(row.getObject(15))))
                    .landMarkType(String.valueOf(row.getObject(16)))
                    .circleLatitude(row.getObject(17) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(17))))
                    .circleLongitude(row.getObject(18) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(18))))
                    .circleRadius(row.getObject(19) == null? 0.0 : Double.valueOf(String.valueOf(row.getObject(19))))
                    .timestamp(System.currentTimeMillis())
                    .build();
        }
        logger.info("Record fetch from database :: {} ",refSchema);
        return Payload.builder().data(Optional.of(Tuple2.of(alertCdc,refSchema))).build();
    }

    @Override
    public void close() throws Exception {
        connection.close();
    }
}
