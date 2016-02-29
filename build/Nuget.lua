local function download(url, file)
  print("Downloading: " .. url)
  local res = http.download(url, file, http.progress)

  if res ~= "OK" then
    os.remove(file)
    error(res)
  end
  return res
end

local function execute(cmd, quiet)
  print(cmd)
  if not quiet then
    return os.execute(cmd)
  else
    local file = assert(io.popen(cmd .. " 2>&1", "r"))
    local output = file:read('*all')
    file:close()
    -- FIXME: Lua 5.2 returns the process exit code from close()
    -- Update this once Premake upgrades from Lua 5.1
    return 0
  end
end

local function execute_or_die(cmd, quiet)
  local res = execute(cmd, quiet)
  if res > 0 then
    error("Error executing shell command, aborting...")
  end
  return res
end

local function download_nuget()
  if not os.isfile("nuget.exe") then
    download("https://nuget.org/nuget.exe", "nuget.exe")
  end
end

local function restore_nuget_packages(cfg, dir)
  local nugetexe = os.is("windows") and "NuGet.exe" or "mono ./NuGet.exe"
  execute_or_die(nugetexe .. " restore " .. cfg .. " -PackagesDirectory " .. dir)
end

local function download_deps()
	download_nuget()
	restore_nuget_packages("../QtSharp/packages.config", "../deps")
end

if _ACTION == "download_deps" then
  download_deps()
  os.exit()
end
