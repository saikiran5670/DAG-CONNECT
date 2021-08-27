package net.atos.daf.ct2.cache.postgres.impl;

import org.apache.flink.api.java.utils.ParameterTool;
import org.apache.flink.configuration.Configuration;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;
import org.mockito.runners.MockitoJUnitRunner;
import org.powermock.modules.junit4.PowerMockRunner;

import java.io.File;
import java.sql.Connection;
import java.sql.DriverManager;

import static net.atos.daf.ct2.props.AlertConfigProp.*;
import static net.atos.daf.ct2.props.AlertConfigProp.MASTER_POSTGRES_SSL;
import static org.junit.Assert.*;
import static org.mockito.Matchers.anyString;
import static org.powermock.api.mockito.PowerMockito.*;


public class RichPostgresMapImplTest {

    @Test
    public void open() throws Exception {
    }

    @Test
    public void map() {
    }

    @Test
    public void close() {
    }
}