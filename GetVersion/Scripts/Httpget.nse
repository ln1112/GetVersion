local http = require "http"
local shortport = require "shortport"
local stdnse = require "stdnse"
local table = require "table"
local string = require "string"



portrule =  shortport.http

action = function(host,port)
    local path
    local response
    local version
    local result = {}

path = stdnse.get_script_args('http-get.path') or '/'

 stdnse.print_debug("%s: %s GET1111!!!!11111 %s",
                       SCRIPT_NAME,
                       host.targetname or host.ip,
                       path)

 response = http.get(host, port.number, path)

 if not response.status then
 stdnse.print_debug("%s: %s GET222222!!!!!!22222222 %s - REQUEST FAILED",
                           SCRIPT_NAME,
                           host.targetname or host.ip,
                           path)
        -- Exit
        return
 end

   if response.status == 404 then
        -- Great success
        stdnse.print_debug("%s: %s GET33333!!!!!!!333333333 %s - 404",
                           SCRIPT_NAME,
                           host.targetname or host.ip,
                           path)


		  stdnse.print_debug("MY BODY!!!!!!!!1111  %s    Charset %s",response.body, response.header["content-type"])

		  version = string.match(response.body, "<h3>Apache Tomcat/([^<]*)</h3>")
        
		stdnse.print_debug("VERSION ---  %s",version)
		
                result = stdnse.output_table()
                result["Server Version"]= version
		return result



   else
      stdnse.print_debug("%s: %s GET444444!!!!!!44444444 %s - %d", 
                           SCRIPT_NAME,
                           host.targetname or host.ip,
                           path,
                           response.status)
   end

end

