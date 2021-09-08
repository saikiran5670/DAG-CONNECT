package net.atos.daf.ct2.cache.postgres.impl;

import net.atos.daf.ct2.cache.postgres.TableStream;
import net.atos.daf.ct2.models.Alert;
import net.atos.daf.ct2.models.Payload;
import net.atos.daf.ct2.models.schema.AlertUrgencyLevelRefSchema;
import net.atos.daf.ct2.models.schema.VehicleAlertRefSchema;
import org.apache.flink.api.common.eventtime.WatermarkStrategy;
import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.common.typeinfo.TypeInformation;
import org.apache.flink.api.java.io.jdbc.JDBCInputFormat;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.typeutils.RowTypeInfo;
import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.connector.jdbc.JdbcConnectionOptions;
import org.apache.flink.connector.jdbc.JdbcExecutionOptions;
import org.apache.flink.connector.jdbc.JdbcSink;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.streaming.api.datastream.DataStreamSource;
import org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.types.Row;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.Serializable;
import java.time.Duration;
import java.util.Optional;

import static net.atos.daf.ct2.props.AlertConfigProp.*;


public class JdbcFormatTableStream extends TableStream<Row> implements Serializable {

    private static final long serialVersionUID = 1637717303256833931L;
    private static final Logger logger = LoggerFactory.getLogger(JdbcFormatTableStream.class);

    public JdbcFormatTableStream(final StreamExecutionEnvironment env,ParameterTool parameters){
        super(env,parameters);
    }

    @Override
    public DataStreamSource<Row> scanTable(String fetchQuery, TypeInformation<?>[] typeInfo,String jdbcUrl) {
        RowTypeInfo rowTypeInfo = new RowTypeInfo(typeInfo);
        JDBCInputFormat jdbcInputFormat = JDBCInputFormat.buildJDBCInputFormat()
                .setDrivername(parameters.get(DRIVER))
                .setDBUrl(jdbcUrl)
                .setQuery(fetchQuery)
                .setRowTypeInfo(rowTypeInfo)
                .finish();
        return env.createInput(jdbcInputFormat);
    }

    @Override
    public void saveAlertIntoDB(DataStream<Alert> alertFoundStream) {
        /**
         * Store into alert db
         */
        String jdbcInsertUrl = new StringBuilder("jdbc:postgresql://")
                .append(parameters.get(DATAMART_POSTGRES_HOST))
                .append(":" + parameters.get(DATAMART_POSTGRES_PORT) + "/")
                .append(parameters.get(DATAMART_DATABASE))
                .append("?user=" + parameters.get(DATAMART_USERNAME))
                .append("&password=" + parameters.get(DATAMART_PASSWORD))
                .append("&sslmode="+parameters.get(DATAMART_POSTGRES_SSL))
                .toString();

        alertFoundStream
                .addSink(JdbcSink.sink(
                        parameters.get("postgres.insert.into.alerts"),
                        (ps, a) -> {
                            ps.setString(1, a.getTripid());
                            ps.setString(2, a.getVin());
                            ps.setString(3, a.getCategoryType());
                            ps.setString(4, a.getType());
                            ps.setLong(5, Long.valueOf(a.getAlertid()));
                            ps.setLong(6, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setLong(7, Long.valueOf(a.getAlertGeneratedTime()));
                            ps.setString(8, a.getUrgencyLevelType());
                        },
                        JdbcExecutionOptions.builder()
                                .withMaxRetries(0)
                                .withBatchSize(1)
                                .build(),
                        new JdbcConnectionOptions.JdbcConnectionOptionsBuilder()
                                .withUrl(jdbcInsertUrl)
                                .withDriverName(parameters.get("driver.class.name"))
                                .build()));
    }

    @Deprecated
    @Override
    public DataStream<Payload>joinTable(DataStreamSource<Row> first, DataStreamSource<Row> second) {

        SingleOutputStreamOperator<AlertUrgencyLevelRefSchema> alertUrgencyStream = first.map(row ->  AlertUrgencyLevelRefSchema.builder()
                .alertId(Long.valueOf(String.valueOf(row.getField(0))))
                .alertCategory(String.valueOf(row.getField(1)))
                .alertType(String.valueOf(row.getField(2)))
                .alertState(String.valueOf(row.getField(3)))
                .urgencyLevelType(String.valueOf(row.getField(4)))
                .thresholdValue(row.getField(5) == null ? 0.0 : Double.valueOf(String.valueOf(row.getField(5))))
                .unitType(String.valueOf(row.getField(6)))
                .timestamp(System.currentTimeMillis())
                .build()
        )
                .returns(AlertUrgencyLevelRefSchema.class)
                .keyBy(alertUrgencyLevelRefSchema -> alertUrgencyLevelRefSchema.getAlertId())
                .assignTimestampsAndWatermarks(
                        WatermarkStrategy
                                .<AlertUrgencyLevelRefSchema>forBoundedOutOfOrderness(Duration.ofSeconds(60))
                                .withTimestampAssigner((alertUrgencyLevelRefSchema, l) -> alertUrgencyLevelRefSchema.getTimestamp())
                );

        SingleOutputStreamOperator<VehicleAlertRefSchema> vehicleMapStream = second.map(row -> new VehicleAlertRefSchema()
                .withAlertId(Long.valueOf(String.valueOf(row.getField(2))))
                .withVin(String.valueOf(row.getField(1)))
                .withState("A")
        )
                .returns(VehicleAlertRefSchema.class)
                .keyBy(vehicleAlertRefSchema -> vehicleAlertRefSchema.getAlertId())
                .assignTimestampsAndWatermarks(
                        WatermarkStrategy
                                .<VehicleAlertRefSchema>forBoundedOutOfOrderness(Duration.ofSeconds(60))
                                .withTimestampAssigner((vehicleAlertRefSchema, l) -> vehicleAlertRefSchema.getTimestamp()));

        DataStream<Payload> joinStream = vehicleMapStream.join(alertUrgencyStream)
                .where(VehicleAlertRefSchema::getAlertId)
                .equalTo(AlertUrgencyLevelRefSchema::getAlertId)
                .window(TumblingEventTimeWindows.of(Time.seconds(60)))
                .apply(
                        new JoinFunction<VehicleAlertRefSchema, AlertUrgencyLevelRefSchema, Payload>() {
                            @Override
                            public Payload join(VehicleAlertRefSchema vehicleAlertRefSchema, AlertUrgencyLevelRefSchema alertUrgencyLevelRefSchema) throws Exception {
                                logger.trace("alert and vin joined from database entity :: {}",vehicleAlertRefSchema);
                                return Payload.builder().data(Optional.of(Tuple2.of(vehicleAlertRefSchema,alertUrgencyLevelRefSchema))).build();
                            }
                        }
                )
                .keyBy(payload -> ((VehicleAlertRefSchema)((Tuple2)payload.getData().get()).f0).getAlertId());

        return joinStream;
    }
    

}
