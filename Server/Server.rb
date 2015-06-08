#!/usr/bin/env ruby
require 'eventmachine'
require 'websocket-eventmachine-server'
require 'yaml'
PORT = (ARGV.shift || 42788).to_i

class Numeric
  def clamp min, max
    [[self, max].min, min].max
  end
end

class String
  def to_bool
    return true   if self == 'true'   || self == 'True' || self =~ (/(true|t|yes|y|1)$/i)
    return false  if self == 'false'  || self == 'False' || self.blank? || self =~ (/(false|f|no|n|0)$/i)
    raise ArgumentError.new("invalid value for Boolean: \"#{self}\"")
  end
end

class UserFile
  attr_accessor :name
  attr_accessor :deviceID
  attr_accessor :TutorialLevelIdx
  attr_accessor :GetGem
  attr_accessor :cashAmount
  attr_accessor :roseCount
  attr_accessor :currentMazeRandSeedCache
  attr_accessor :currentMazeLevel
  attr_accessor :safeBoxDatas
  attr_accessor :tricksBought
  attr_accessor :guardsHired
  attr_accessor :guardsSummoned
  attr_accessor :slotStatus
  attr_accessor :isBot
  attr_accessor :Buildings
  attr_accessor :defReplays
  attr_accessor :atkReplays
  attr_accessor :beenStealingTimeStamp
  attr_accessor :PvEProgress
  def initialize(roseBuildingDuration, bornNewTargetDuration)
    @name = ""
    @deviceID = "-1"
    @TutorialLevelIdx="GetGem"
    @cashAmount = 0.0
    @roseCount=0
    @currentMazeRandSeedCache=Random.new_seed%10000000
    @currentMazeLevel=0
    @safeBoxDatas=["0","0"]
    @tricksBought=[""]
    @guardsHired=[]
    @guardsSummoned=""
    @slotStatus=["0","-1","-1"]
    @isBot= false
    @Buildings=[Building.new("1","None", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("2","None", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("3","None", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("4","None", roseBuildingDuration, bornNewTargetDuration)]
    @defReplays=[]
    @atkReplays=[]
    @beenStealingTimeStamp=Time.new(1988)
    @PvEProgress=0
  end
end

class Building
  attr_accessor :posID
  attr_accessor :type
  attr_accessor :unpickedRose
  attr_accessor :everClickedTarget
  attr_accessor :newTargetTimeStamp
  attr_accessor :roseGrowBeginTimeStamp
  attr_accessor :roseGrowTotalDuration
  attr_accessor :roseGrowLastDuration
  attr_accessor :roseGrowTimer
  attr_accessor :roseGrowStopTimer
  attr_accessor :sendNewTargetTimer
  attr_accessor :bornNewTargetLastDuration
  attr_accessor :targetName
  attr_accessor :isPvP
  attr_accessor :botLevelRandSeed
  attr_accessor :PvELevelIdx

  def initialize(id, t, roseDuration, newTargetDuration)
    @posID = id
    @type = t
    @unpickedRose = 1
    @newTargetTimeStamp = Time.new(1988)
    @roseGrowBeginTimeStamp = Time.now
    @roseGrowTimer = nil
    @roseGrowStopTimer = nil
    @sendNewTargetTimer = nil
    @everClickedTarget = false
    @roseGrowLastDuration = roseDuration
    @roseGrowTotalDuration = roseDuration
    @bornNewTargetLastDuration = newTargetDuration
    @targetName = "poker_face"
    @isPvP = false
    @botLevelRandSeed = -1
    @PvELevelIdx = -1
  end
end

class ReplayData
  attr_accessor :date
  attr_accessor :StealingCash
  attr_accessor :ini
  attr_accessor :everClicked
  attr_accessor :isPvP
  attr_accessor :thief
  attr_accessor :guard
end

class Player
  attr_accessor :userFile
  attr_accessor :seperator
  attr_accessor :websocket
  def initialize(ws)
    @websocket = ws
    @seperator = '|'
    @timers = []
    #偷满是5朵玫瑰
    @roseBuildingDuration = 62
    @roseGrowCycle = 0.2*60
    @bornNewTargetDuration = 60
    @userFile = UserFile.new(@roseBuildingDuration,@bornNewTargetDuration)
  end

  def send(msg)
    EM.add_timer(0.1) do
      @websocket.send(msg)
      puts 'send ' + msg
    end
  end

  def ReadPlayerFile(filename)
    file = File.new(filename)
    cnf = YAML.load(file)
    file.close
    return cnf
  end

  def Login(protocal_no,contents)
    userID = contents[0].force_encoding('utf-8')
    deviceID = contents[1]
    if(File.exist?(userID+".yaml"))
      @userFile = ReadPlayerFile(userID+".yaml")
      if(deviceID == @userFile.deviceID)
        send(protocal_no + @seperator + "ok")
      else
        send(protocal_no + @seperator + "duplicated")
      end
    else
      send(protocal_no + @seperator + "ok")
      @userFile.name = userID
      @userFile.deviceID = deviceID
      SaveToFile(userID+".yaml", @userFile)
    end
  end

  def SelfStealingInfo(protocal_no,contents)
    send(protocal_no + @seperator + StealingInfo(@userFile))
  end

  def StealingInfo(playerFile)
    reply = playerFile.TutorialLevelIdx + "&" +
        playerFile.cashAmount.to_s + "&" +
        playerFile.roseCount.to_s + "&" +
        playerFile.currentMazeLevel.to_s + "&" +
        playerFile.currentMazeRandSeedCache.to_s
    reply += "&"
    playerFile.slotStatus.each{ |slotStatu| reply += "," +"#{slotStatu}"}
    reply += "&"
    playerFile.safeBoxDatas.each{ |boxLv| reply += "," + "#{boxLv}"}
    reply += "&"
    playerFile.guardsHired.each{ |guard| reply += "," + "#{guard}"}
    reply += "&" + playerFile.guardsSummoned.to_s
    reply += "&"
    playerFile.tricksBought.each{ |trickname| reply += "," + "#{trickname}"}
    reply += "&" + playerFile.isBot.to_s
    reply += "&" + playerFile.name
    reply += "&" + playerFile.PvEProgress.to_s
    reply
  end

  def UpgradeMaze(protocal_no,contents)
    @userFile.currentMazeLevel = contents[0].to_i
    @userFile.currentMazeRandSeedCache = contents[1].to_i
  end

  def TrickBought(protocal_no,contents)
    @userFile.tricksBought << contents[0]
  end

  def UsingTrick(protocal_no,contents)
    trickname = contents[0]
    slotIdx = contents[1].to_i
    @userFile.slotStatus[slotIdx] = trickname
  end

  def UnuseTrick(protocal_no,contents)
    trickname = contents[0]
    slotIdx = contents[1].to_i
    @userFile.slotStatus[slotIdx] = "0"
  end

  def SlotBought(protocal_no,contents)
    slotIdx = contents[0].to_i
    @userFile.slotStatus[slotIdx] = "0"
  end

  def Cash(protocal_no,contents)
    @userFile.cashAmount = contents[0].to_f
  end

  def PickRose(protocal_no,contents)
    @userFile.roseCount += contents[0].to_i
    # 收集玫瑰的时候，检查RoseBuilding的时间是否已经到了
    posID = contents[1]
    building = GetBuilding(posID)
    if building != nil
      building.unpickedRose = 0
      #剩余时间
      growLastDuration = building.roseGrowTotalDuration - (Time.now - building.roseGrowBeginTimeStamp)
      if growLastDuration < @roseGrowCycle
        SendNone(building, @bornNewTargetDuration)
      end
    end
  end

  def AddSafebox(protocal_no,contents)
    @userFile.safeBoxDatas << "0"
  end

  def UpgradeSafebox(protocal_no,contents)
    idx = contents[0].to_i
    lv = @userFile.safeBoxDatas[idx].to_i
    lv += 1
    puts lv
    @userFile.safeBoxDatas[idx] = lv.to_s
  end

  def HireGuard(protocal_no,contents)
    @userFile.guardsHired << contents[0]
  end

  def UploadSummonedGuards(protocal_no,contents)
    @userFile.guardsSummoned = contents[0]
  end

  def ClickTarget(protocal_no,contents)
    posID = contents[0]
    building = GetBuilding(posID)
    building.everClickedTarget = true
  end

  def GetBuilding(posID)
    @userFile.Buildings.each{ |building|
      if posID == building.posID
        return building
      end
    }
    nil
  end

  def GetTargetBuilding(target)
    @userFile.Buildings.each{ |building|
      return building if (building.type == "Target" && building.targetName == target)
    }
    return nil
  end

  def HasTargetBuilding(targetName)
    return true if GetTargetBuilding(targetName) != nil
    return false
  end

  def ClickReplay(protocal_no,contents)
    date = contents[0]
    @userFile.defReplays.each{ |replay|
      if date == replay.date
        replay.everClicked = "True"
        return
      end
    }
    @userFile.atkReplays.each{ |replay|
      if date == replay.date
        replay.everClicked = "True"
        return
      end
    }
  end

  def SendNewTarget(building)
    #随机选择一份敌人文档
    enemies = []
    if @userFile.PvEProgress > 2 && (Random.rand(0.0...1.0) > 0.5)
      Dir.glob("*.yaml") do |player_achive_file|
        player_achive_file = player_achive_file.encode('UTF-8')
        enemyUserFile = ReadPlayerFile(player_achive_file)
        enemies << enemyUserFile.name if (
        !HasTargetBuilding(enemyUserFile.name) &&
            enemyUserFile.name != @userFile.name &&
            (enemyUserFile.roseCount - @userFile.roseCount).abs < 10 &&
            Time.now - enemyUserFile.beenStealingTimeStamp > 10 * 60 &&
            enemyUserFile.TutorialLevelIdx == "Over")
      end
    end

    if enemies.length != 0
      building.targetName = enemies[Random.rand(0..(enemies.length-1))]
      building.isPvP = true
      building.PvELevelIdx = -1
    else
      targetNames = [
          "poker_face","cash_eye","cat_eye_lady","kaitokid","magic_thief",
          "silver_wing","staring","under_moon","in_the_shadow","quitely",
          "gambling","crap_game","cover_truth","secret_keeper","now_you_see_me" ,
          "COC","boom_beach","spy_mouse","VR","mark_of_the_ninja","stealth_inc"]
      building.targetName = targetNames[Random.rand(0..(targetNames.length-1))]
      building.isPvP = false
      building.botLevelRandSeed = Random.new_seed%10000000
      building.PvELevelIdx = @userFile.PvEProgress
      @userFile.PvEProgress = @userFile.PvEProgress + 1
    end

    building.type = "Target"
    building.newTargetTimeStamp = Time.new(1988)
    building.bornNewTargetLastDuration = 0
    building.sendNewTargetTimer = nil
    building.everClickedTarget = false

    send("new_target" +
                        seperator + building.posID +
                        seperator + building.targetName +
                        seperator + building.isPvP.to_s +
                        seperator + building.botLevelRandSeed.to_s +
                        seperator + building.PvELevelIdx.to_s +
                        seperator + @userFile.PvEProgress.to_s)
  end

  def SendNone(building, newTargetDuration)
    building.type = "None"
    building.roseGrowBeginTimeStamp = -1
    building.roseGrowLastDuration = 0
    building.bornNewTargetLastDuration = newTargetDuration
    building.targetName = ""
    send("roseBuildingEnd" + @seperator + building.posID)
    building.newTargetTimeStamp = Time.now
    building.sendNewTargetTimer = EM.add_timer(@bornNewTargetDuration) do
      SendNewTarget(building)
    end
  end

  def RoseGrowBegin(building)
    timeElapsed = Time.now - building.roseGrowBeginTimeStamp
    #计算逝去的时间内长出了几朵玫瑰
    roseDelta = 0
    if building.roseGrowLastDuration > 0
      if timeElapsed > building.roseGrowLastDuration
        roseDelta = (building.roseGrowLastDuration/@roseGrowCycle).to_i
      else
        roseDelta = (timeElapsed/@roseGrowCycle).to_i
      end
    end
    #更新剩下的时间
    building.roseGrowLastDuration = building.roseGrowTotalDuration - timeElapsed
    building.unpickedRose += roseDelta
    #如果rosebuilding还能继续生产玫瑰，
    if building.roseGrowLastDuration > 0
      building.roseGrowTimer = EM.add_timer(@roseGrowCycle - timeElapsed%@roseGrowCycle) do
        RoseGrow(building)
      end
      building.roseGrowStopTimer = EM.add_timer(building.roseGrowLastDuration) do
        roseGrowStop(building)
      end
    else
      building.roseGrowLastDuration = 0
    end
  end

  def RoseGrow(building)
    building.unpickedRose += 1
    send("rose_grow" + seperator + building.posID)
    building.roseGrowTimer = EM.add_timer(@roseGrowCycle) do
      RoseGrow(building)
    end
  end

  def roseGrowStop(building)
    EM.cancel_timer(building.roseGrowTimer)
    building.roseGrowTimer = nil
    building.roseGrowStopTimer = nil
    building.roseGrowLastDuration = building.roseGrowTotalDuration - (Time.now - building.roseGrowBeginTimeStamp)
  end

  def DownloadBuildings(protocal_no,contents)
    @userFile.Buildings.each{ |building|
      case building.type
        # 检查RoseBuilding是否该生成新的玫瑰
        when "Rose"
          RoseGrowBegin(building)
        # 检查NoneBuilding是否该生成新的Target
        when "None"
          timeElapsed = Time.now - building.newTargetTimeStamp
          building.bornNewTargetLastDuration = @bornNewTargetDuration - timeElapsed
          if building.bornNewTargetLastDuration < 0
            SendNewTarget(building)
          else
            building.sendNewTargetTimer = EM.add_timer(building.bornNewTargetLastDuration) do
              SendNewTarget(building)
            end
          end
      end
      send("building" +
                          @seperator + building.posID +
                          @seperator + building.type +
                          @seperator + building.unpickedRose.to_s +
                          @seperator + building.everClickedTarget.to_s +
                          @seperator + building.targetName +
                          @seperator + building.isPvP.to_s +
                          @seperator + building.botLevelRandSeed.to_s +
                          @seperator + building.PvELevelIdx.to_s)
    }
    send("buildings_ready")
  end

  def DownloadReplays(protocal_no,contents)
    reply = protocal_no
    @userFile.defReplays.each{ |replay|
      send("replay" + @seperator + PackReplay(replay))
    }
    send("replays_ready")
  end

  def PackReplay(replay)
    returnStr = replay.date +
        @seperator + replay.StealingCash+
        @seperator + replay.ini+
        @seperator + replay.everClicked+
        @seperator + replay.thief+
        @seperator + replay.guard
    returnStr
  end

  def StealingOver(protocal_no,contents, enemy)
    replay = ReplayData.new()
    replay.date = contents[0]
    replay.StealingCash = contents[1]
    replay.ini = contents[2]
    replay.everClicked = contents[3]
    replay.thief = StealingInfo(@userFile)
    replay.guard = StealingInfo(enemy.userFile)
    bIsPerfectStealing = contents[4].to_bool
    enemyCashBeforeSteal = 1.0
    # 如果是pvp，要扣除对方金钱
    if !enemy.userFile.isBot
      enemyCashBeforeSteal = enemy.userFile.cashAmount
      enemy.userFile.cashAmount = (enemyCashBeforeSteal - replay.StealingCash.to_f).to_s
      enemy.userFile.beenStealingTimeStamp = Time.now
      enemy.userFile.defReplays << replay
      enemy.userFile.defReplays.shift if enemy.userFile.defReplays.length > 5
      SaveToFile(enemy.userFile.name + ".yaml", enemy.userFile)
    else
      # bot的金钱是客户端上传的
      enemy.userFile.cashAmount = contents[5].to_f
      enemyCashBeforeSteal = contents[5].to_f
    end

    @userFile.atkReplays << replay
    @userFile.atkReplays.shift if @userFile.atkReplays.length > 5

    send("atk_replay" + seperator + PackReplay(replay))
    stolen_cash_ratio =  (replay.StealingCash.to_f / enemyCashBeforeSteal).clamp(0.0, 1.0)
    buildingPosID = contents[6]
    building = GetBuilding(buildingPosID)
    building.roseGrowTotalDuration = @roseBuildingDuration * stolen_cash_ratio
    # 如果是玩家，偷窃0.2以上，刷新成收玫瑰，否则更换目标
    if !enemy.userFile.isBot
      if stolen_cash_ratio > 0.4
        building.type = "Poor"
        send("new_poor" + seperator + buildingPosID)
      else
        SendNone(building, @bornNewTargetDuration * 2)
      end

    else #如果是Bot，必须完全偷完才刷新成玫瑰
      if bIsPerfectStealing
        building.type = "Poor"
        send("new_poor" + seperator + buildingPosID)
      end
    end

  end

  def TurnToRoseBuilding(protocal_no,contents)
    buildingPosID = contents[0]
    building = GetBuilding(buildingPosID)
    building.unpickedRose = 0
    building.type = "Rose"
    building.roseGrowBeginTimeStamp = Time.now
    RoseGrowBegin(building)
    send("new_rosebuilding" + seperator + buildingPosID)
  end

  def AdvanceTutorial(protocal_no,contents)
    @userFile.TutorialLevelIdx = contents[0]
  end

  def DownloadRanks(protocal_no,contents)
    players = []
    Dir.glob("*.yaml") do |player_achive_file|
      players << ReadPlayerFile(player_achive_file)
    end
    players.sort! {|x, y| x.roseCount <=> y.roseCount}
    players.each{ |player|
      send("download_one_rank" + seperator + player.name + seperator + player.roseCount.to_s)
      }
    send("download_ranks_over")
  end

  def DownloadOtherReplays(protocal_no,contents)
    other = nil
    name = contents[0].force_encoding('utf-8')
    Dir.glob("*.yaml") do |player_achive_file|
      other = ReadPlayerFile(player_achive_file)
      if name == other.name
        break
      end
    end
    other.atkReplays.each{ |replay|
      send("download_one_other_replay" + @seperator + PackReplay(replay))
    }
    send("download_other_replays_over")
  end

  def Close
    if(@userFile != nil)
      @userFile.Buildings.each{ |building|
        if building.roseGrowTimer != nil
          EM.cancel_timer(building.roseGrowTimer)
          building.roseGrowTimer = nil
        end
        if building.roseGrowStopTimer != nil
          roseGrowStop(building)
        end
        if building.sendNewTargetTimer != nil
          EM.cancel_timer(building.sendNewTargetTimer)
          building.sendNewTargetTimer = nil
          building.bornNewTargetLastDuration = @bornNewTargetDuration - (Time.now - building.newTargetTimeStamp)
        end
      }
      SaveToFile(@userFile.name + ".yaml", @userFile)
    end
  end

  def SaveToFile(filename, userfile)
    file = File.new(filename,"w+:UTF-8")
    file.write(userfile.to_yaml)
    file.close
  end
end

EM::run do
  @channel = EM::Channel.new
  puts "start websocket server - port:#{PORT}"
  WebSocket::EventMachine::Server.start(:host => "0.0.0.0", :port => PORT) do |ws|
    ws.onopen do
        sid = @channel.subscribe do |mes|
          ws.send mes
        end
        puts "<#{sid}> connect"
        @channel.push "hello new client <#{sid}>"
        @player = Player.new(ws)
        @enemy = nil
        ws.onmessage do |msg|
          begin
          contents = msg.to_s.split(@player.seperator)
          puts "<#{sid}>" + contents.to_s
          protocal_no = contents[0]
          contents.shift
          case protocal_no
            when "login"
              @player.Login(protocal_no,contents)
            when "self_stealing_info"
              @player.SelfStealingInfo(protocal_no,contents)
            when "upgrade_maze"
              @player.UpgradeMaze(protocal_no,contents)
            when "using_trick"
              @player.UsingTrick(protocal_no,contents)
            when "unuse_trick"
              @player.UnuseTrick(protocal_no,contents)
            when "slot_bought"
              @player.SlotBought(protocal_no,contents)
            when "trick_bought"
              @player.TrickBought(protocal_no,contents)
            when "download_buildings"
              @player.DownloadBuildings(protocal_no,contents)
            when "download_replays"
              @player.DownloadReplays(protocal_no,contents)
            when "download_target"
              @enemy = Player.new(ws)
              targetName = contents[0]
              isBot = contents[1].to_bool
              buildingPosID = contents[2]
              if !isBot
                player_achive_file = player_achive_file
                @enemy.userFile = @enemy.ReadPlayerFile((targetName + ".yaml").force_encoding('utf-8'))
                if @enemy.userFile.cashAmount < 1000
                  @enemy.userFile.cashAmount = 5000
                end
              else
                @enemy.userFile.name = targetName
                @enemy.userFile.TutorialLevelIdx = "Over"
                @enemy.userFile.isBot = true
                @enemy.userFile.currentMazeRandSeedCache = @player.GetBuilding(buildingPosID).botLevelRandSeed
                @enemy.userFile.currentMazeLevel = @player.userFile.currentMazeLevel
              end
              @enemy.websocket.send("download_target" + @enemy.seperator + @enemy.StealingInfo(@enemy.userFile))

            when "stealing_over"
              @player.StealingOver(protocal_no,contents, @enemy)
              @enemy = nil
            when "cash"
              @player.Cash(protocal_no,contents)
            when "pick_rose"
              @player.PickRose(protocal_no,contents)
            when "add_safebox"
              @player.AddSafebox(protocal_no,contents)
            when "upgrade_safebox"
              @player.UpgradeSafebox(protocal_no,contents)
            when "hire_guard"
              @player.HireGuard(protocal_no,contents)
            when "upload_summoned_guards"
              @player.UploadSummonedGuards(protocal_no,contents)
            when "click_target"
              @player.ClickTarget(protocal_no,contents)
            when "click_replay"
              @player.ClickReplay(protocal_no,contents)
            when "poor_turn_to_rose"
              @player.TurnToRoseBuilding(protocal_no,contents)
            when "advance_tutorial"
              @player.AdvanceTutorial(protocal_no,contents)
            when "download_ranks"
              @player.DownloadRanks(protocal_no,contents)
            when "download_other_replays"
              @player.DownloadOtherReplays(protocal_no,contents)
          end
          rescue Exception => e
            file = File.new('ErrorLogs.txt',"a+:UTF-8")
            file.write(@player.userFile.name) if @player.userFile != nil
            file.write("\n")
            file.write(Time.now.to_s)
            file.write("\n")
            file.write(e.backtrace.join("\n"))
            file.write(e.message)
            file.write("\n")
            file.write("\n")
            file.write("\n")
            file.close
            puts e.backtrace
            puts e.message
            ws.close
          end
        end
        ws.onclose do
          @player.Close
          puts "<#{sid}> disconnected"
          @channel.unsubscribe sid
          @channel.push "<#{sid}> disconnected"
        end
      end
  end
end