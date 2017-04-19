CREATE OR REPLACE STREAM DESTINATION_SQL_STREAM (
    ITEM VARCHAR(1024),
    ITEM_COUNT DOUBLE);

-- Top API calls
CREATE OR REPLACE PUMP "STREAM_PUMP" AS
    INSERT INTO "DESTINATION_SQL_STREAM"
        SELECT STREAM *
        FROM TABLE (TOP_K_ITEMS_TUMBLING(CURSOR(SELECT STREAM * FROM "SOURCE_SQL_STREAM_001"), 'method_name', 5, 60));


--- Anomaly detection
--Creates a temporary stream.
CREATE OR REPLACE STREAM "ANOMALY_TEMP_STREAM" (
       method_name VARCHAR(256),
       elapsed_ms INTEGER,
       timestamp_dt TIMESTAMP,
       customer_id int,
       ANOMALY_SCORE DOUBLE);

--Creates another stream for application output.	        
CREATE OR REPLACE STREAM "ANOMALY_DESTINATION_SQL_STREAM" (
       method_name VARCHAR(256),
       elapsed_ms INTEGER,
       timestamp_dt TIMESTAMP,
       customer_id int,
       ANOMALY_SCORE DOUBLE);

-- Compute an anomaly score for each record in the input stream using Random Cut Forest
CREATE OR REPLACE PUMP "ANOMALY_STREAM_PUMP" AS 
   INSERT INTO "ANOMALY_TEMP_STREAM"
      SELECT STREAM "method_name", "elapsed_ms", "timestamp_dt", "customer_id", ANOMALY_SCORE 
      FROM TABLE(RANDOM_CUT_FOREST(
              CURSOR(SELECT STREAM * FROM "SOURCE_SQL_STREAM_001")));

-- Sort records by descending anomaly score, insert into output stream
CREATE OR REPLACE PUMP "ANOMALY_OUTPUT_PUMP" AS 
   INSERT INTO "ANOMALY_DESTINATION_SQL_STREAM"
      SELECT STREAM * FROM "ANOMALY_TEMP_STREAM"
      ORDER BY FLOOR("ANOMALY_TEMP_STREAM".ROWTIME TO SECOND), ANOMALY_SCORE DESC;
