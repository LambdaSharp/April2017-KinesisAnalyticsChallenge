-- Top API calls
CREATE OR REPLACE STREAM DESTINATION_SQL_STREAM;
CREATE OR REPLACE PUMP "STREAM_PUMP";


--- Anomaly detection
CREATE OR REPLACE STREAM "ANOMALY_TEMP_STREAM";

--Creates another stream for application output.	        
CREATE OR REPLACE STREAM "ANOMALY_DESTINATION_SQL_STREAM";

-- Compute an anomaly score for each record in the input stream using Random Cut Forest
CREATE OR REPLACE PUMP "ANOMALY_STREAM_PUMP";

-- Sort records by descending anomaly score, insert into output stream
CREATE OR REPLACE PUMP "ANOMALY_OUTPUT_PUMP";
