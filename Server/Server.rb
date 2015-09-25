#!/usr/bin/env ruby
require 'eventmachine'
require 'websocket-eventmachine-server'
require 'yaml'
PORT = 42788
$onlinePlayers = []
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

class MagicianData
  attr_accessor :name
  attr_accessor :strengthAllot
  attr_accessor :agilityAllot
  attr_accessor :wisdomAllot
  def initialize(nameStr)
    @name = nameStr
    @strengthAllot = 0
    @agilityAllot = 0
    @wisdomAllot = 0
  end
end


class TrickData
  attr_accessor :name
  attr_accessor :learned
  attr_accessor :inventory
  attr_accessor :slotIdxInUsingPanel
  def initialize(nameStr, learn, inventroyNum)
    @name = nameStr
    @learned = learn
    @inventory = inventroyNum
    @slotIdxInUsingPanel = -1
  end
end

class UserFile
  attr_accessor :name
  attr_accessor :deviceID
  attr_accessor :TutorialLevelIdx
  attr_accessor :GetGem
  attr_accessor :cashAmount
  attr_accessor :roseCount
  attr_accessor :roseLast
  attr_accessor :currentMazeRandSeedCache
  attr_accessor :currentMazeLevel
  attr_accessor :safeBoxDatas
  attr_accessor :guardsHired
  attr_accessor :guardsSummoned
  attr_accessor :slotStatus
  attr_accessor :isBot
  attr_accessor :Buildings
  attr_accessor :Clouds
  attr_accessor :defReplays
  attr_accessor :atkReplays
  attr_accessor :beenStealingTimeStamp
  attr_accessor :logOutTimeStamp
  attr_accessor :PvEProgress
  attr_accessor :Magicians
  attr_accessor :selectedMagician
  attr_accessor :Clouds
  attr_accessor :trickDatas
  attr_accessor :droppedItemsFromThief
  def initialize(roseBuildingDuration, bornNewTargetDuration)
    @name = ""
    @deviceID = "-1"
    @TutorialLevelIdx="GetGem"
    @cashAmount = 0.0
    @roseCount=0
    @roseLast = @roseCount
    @currentMazeRandSeedCache=Random.new_seed%10000000
    @currentMazeLevel=0
    @safeBoxDatas=["0","0"]
    @guardsHired=[]
    @guardsSummoned=""
    @slotStatus=["0","-1","-1"]
    @isBot= false
    @Buildings=[Building.new("0","None", roseBuildingDuration, bornNewTargetDuration),
                Building.new("1","None", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("2","None", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("3","locked", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("4","locked", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("5","locked", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("6","locked", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("7","locked", roseBuildingDuration, bornNewTargetDuration),
                  Building.new("8","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("9","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("10","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("11","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("12","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("13","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("14","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("15","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("16","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("17","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("18","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("19","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("20","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("21","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("22","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("23","locked", roseBuildingDuration, bornNewTargetDuration),
                Building.new("24","locked", roseBuildingDuration, bornNewTargetDuration)]
    @Clouds=[CloudData.new(0, true),
             CloudData.new(1, true),
             CloudData.new(2, true),
             CloudData.new(3, true),
             CloudData.new(4, true),
             CloudData.new(5, true),
             CloudData.new(6, true),
             CloudData.new(7, true),
             CloudData.new(8, true)]

    @defReplays=[]
    @atkReplays=[]
    @beenStealingTimeStamp=Time.new(1988)
    @PvEProgress=0
    @logOutTimeStamp=Time.now
    @Magicians = [MagicianData.new("Rosa"),
                  MagicianData.new("Mine Fujiko")]
    @selectedMagician = "Rosa"

    @trickDatas=[TrickData.new("hypnosis", false,10),
                 TrickData.new("disguise", false,0),
                 TrickData.new("dove", false,0),
                 TrickData.new("flash_grenade", false,0),
                 TrickData.new("shotLight", false,0),
                 TrickData.new("flyUp", false,0)]
    @droppedItemsFromThief=[]
  end
end

class Building
  attr_accessor :posID
  attr_accessor :type
  attr_accessor :unpickedRose
  attr_accessor :roseGrowedCount
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
    @roseGrowedCount = 0
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
  attr_accessor :rewardAccepted
  attr_accessor :reward_rose_count
  attr_accessor :isPvP
  attr_accessor :thief
  attr_accessor :guard
end

class CloudData
  attr_accessor :idx
  attr_accessor :locked
  def initialize(id, locked)
    @idx = id
    @locked = locked
  end
end

class Player
  attr_accessor :userFile
  attr_accessor :seperator
  attr_accessor :websocket
  def initialize(ws)
    @websocket = ws
    @seperator = '|'
    #偷满是5朵玫瑰
    @roseBuildingDuration = 60 * 20 + 2
    @roseGrowCycle = 60*20 / 9.0
    @roseAddPowerRate = 2.0
    @bornNewTargetDuration = 60*15
    @userFile = UserFile.new(@roseBuildingDuration,@bornNewTargetDuration)
    @punishRoseCount = 3
  end

  def send(msg)
    #EM.add_timer(0.1) do
      @websocket.send(msg)
      puts 'send ' + msg
    #end
  end

  def ReadPlayerFile(filename)
    file = File.new(filename)
    cnf = YAML.load(file)
    file.close
    return cnf
  end

  def Login(protocal_no,contents)
    versionStr = "0.1"
    clientVersionStr = contents[2].force_encoding('utf-8')
    if clientVersionStr == versionStr
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
    else
      send(protocal_no + @seperator + "version_error")
    end
  end

  def SelfStealingInfo(protocal_no,contents)
    send(protocal_no + @seperator + StealingInfo(@userFile))
  end

  def StealingInfo(playerFile)
    reply = playerFile.TutorialLevelIdx + "&" +
        playerFile.cashAmount.to_s + "&" +
        playerFile.roseCount.to_s + "&" +
        playerFile.roseLast.to_s + "&" +
        playerFile.currentMazeLevel.to_s + "&" +
        playerFile.currentMazeRandSeedCache.to_s + "&" +
        playerFile.selectedMagician
    reply += "&"
    playerFile.slotStatus.each{ |slotStatu| reply += "," +"#{slotStatu}"}
    reply += "&"
    playerFile.safeBoxDatas.each{ |boxLv| reply += "," + "#{boxLv}"}
    reply += "&"
    playerFile.guardsHired.each{ |guard| reply += "," + "#{guard}"}
    reply += "&" + playerFile.guardsSummoned.to_s
    reply += "&"

    playerFile.trickDatas.each{ |trick_data|
      reply += trick_data.name.to_s + ","
      reply += trick_data.learned.to_s + ","
      reply += trick_data.inventory.to_s + ","
      reply += trick_data.slotIdxInUsingPanel.to_s + ","
    }

    reply += "&" + playerFile.isBot.to_s
    reply += "&" + playerFile.name
    reply += "&" + playerFile.PvEProgress.to_s
    reply += "&" + @roseGrowCycle.to_s
    reply += "&" + @roseAddPowerRate.to_s
    reply += "&" + @punishRoseCount.to_s
    reply += "&"
    playerFile.Magicians.each{ |mage_data|
      reply += mage_data.name.to_s + ","
      reply += mage_data.strengthAllot.to_s + ","
      reply += mage_data.agilityAllot.to_s + ","
      reply += mage_data.wisdomAllot.to_s + ","
    }
    reply += "&"
    playerFile.droppedItemsFromThief.each{ |item| reply += "," + "#{item}"}
    reply
  end

  def UpgradeMaze(protocal_no,contents)
    @userFile.currentMazeLevel = contents[0].to_i
    @userFile.currentMazeRandSeedCache = contents[1].to_i
  end

  def LearnTrick(protocal_no,contents)
    data = GetTrickData(contents[0].force_encoding('utf-8'))
    data.learned = true
  end

  def UsingTrick(protocal_no,contents)
    trickname = contents[0]
    data = GetTrickData(trickname.force_encoding('utf-8'))
    data.slotIdxInUsingPanel = contents[1].to_i
  end

  def UnuseTrick(protocal_no,contents)
    trickname = contents[0]
    data = GetTrickData(trickname.force_encoding('utf-8'))
    data.slotIdxInUsingPanel = -1
  end

  def AddTrickItem(protocal_no,contents)
    trickname = contents[0]
    data = GetTrickData(trickname.force_encoding('utf-8'))
    data.inventory += 1
  end

  def ConsumeTrickItem(protocal_no,contents)
    trickname = contents[0]
    data = GetTrickData(trickname.force_encoding('utf-8'))
    data.inventory -= 1
  end

  def DropTrickItem(protocal_no,contents,enemy)
    trickname = contents[0]
    data = GetTrickData(trickname.force_encoding('utf-8'))
    data.inventory -= 1
    if enemy != nil && !enemy.userFile.isBot
      enemy.userFile.droppedItemsFromThief << data.name
    end
  end

  def RemoveDroppedItem(protocal_no,contents,player)
    trickname = contents[0]
    player.userFile.droppedItemsFromThief.delete(trickname)
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
    @userFile.roseLast += contents[0].to_i
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

  def ChangeRose(protocal_no,contents)
    @userFile.roseCount += contents[0].to_i
    if @userFile.roseCount < 0
      @userFile.roseCount = 0
    end
  end

  def UploadMagician(protocal_no,contents)
    @userFile.roseLast = contents[0].to_i
    @userFile.Magicians.each { |mage_data|
      if contents[1].force_encoding('utf-8') == mage_data.name
        mage_data.strengthAllot = contents[2].to_i
        mage_data.agilityAllot = contents[3].to_i
        mage_data.wisdomAllot = contents[4].to_i
      end
    }
  end

  def SelectMagician(protocal_no,contents)
    @userFile.selectedMagician = contents[0]
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

  def GetTrickData(name)
    @userFile.trickDatas.each{ |data|
      if name == data.name
        return data
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

  def GetPvEMazeLv(pvELevelIdx)
    mazeLv = case pvELevelIdx
      when 0..4
        1
      when 5..8
        2
      when 9..11
        3
      when 12..15
        5
      else
        6
    end
  end

  def RewardAccepted(protocal_no,contents)
    date = contents[0]
    @userFile.defReplays.each{ |replay|
      if date == replay.date
        replay.rewardAccepted = "True"
        return
      end
    }
  end

  def SendNewTarget(building)
    #随机选择一份敌人文档
    enemies = []
    if @userFile.PvEProgress > 2 && (Random.rand(0.0...1.0) > 0.3)
      $onlinePlayers.each{ |p|
        enemies << p.userFile if (
        !HasTargetBuilding(p.userFile.name) &&
            p.userFile.name != @userFile.name &&
            (p.userFile.roseCount - @userFile.roseCount).abs < 10 &&
            Time.now - p.userFile.beenStealingTimeStamp > 10 * 60 &&
            p.userFile.TutorialLevelIdx == "Over")
      }

      if enemies.length == 0
        Dir.glob("*.yaml") do |player_achive_file|
          player_achive_file = player_achive_file.encode('UTF-8')
          enemyUserFile = ReadPlayerFile(player_achive_file)
          enemies << enemyUserFile if (
          !HasTargetBuilding(enemyUserFile.name) &&
              enemyUserFile.name != @userFile.name &&
              (enemyUserFile.roseCount - @userFile.roseCount).abs < 10 &&
              Time.now - enemyUserFile.beenStealingTimeStamp > 10 * 60 &&
              enemyUserFile.TutorialLevelIdx == "Over")
        end
      end
    end

    mazeLv = -1
    if enemies.length != 0
      enemyFile = enemies[Random.rand(0..(enemies.length-1))]
      building.targetName = enemyFile.name
      building.isPvP = true
      building.PvELevelIdx = -1
      mazeLv = enemyFile.currentMazeLevel
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
      mazeLv = GetPvEMazeLv(building.PvELevelIdx)
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
                        seperator + @userFile.PvEProgress.to_s +
             seperator + mazeLv.to_s)
  end

  def SendNone(building, newTargetDuration)
    building.type = "None"
    building.roseGrowBeginTimeStamp = -1
    building.roseGrowLastDuration = 0
    building.bornNewTargetLastDuration = newTargetDuration
    building.targetName = ""
    send("roseBuildingEnd" + @seperator + building.posID + @seperator + newTargetDuration.to_s)
    building.newTargetTimeStamp = Time.now
    building.sendNewTargetTimer = EM.add_timer(@bornNewTargetDuration) do
      SendNewTarget(building)
    end
  end

  def RoseGrowBegin(building,timeElapsed)
    growing_period = Time.now - building.roseGrowBeginTimeStamp
    growing_period = building.roseGrowTotalDuration if growing_period > building.roseGrowTotalDuration
    rose_growed_count_till_now_should_be = growing_period / @roseGrowCycle

    building.unpickedRose += rose_growed_count_till_now_should_be.to_i - building.roseGrowedCount
    building.roseGrowedCount = rose_growed_count_till_now_should_be.to_i

    #更新剩下的时间
    building.roseGrowLastDuration = building.roseGrowTotalDuration - growing_period

    #如果rosebuilding还能继续生产玫瑰，
    if building.roseGrowLastDuration > 0
      building.roseGrowTimer = EM.add_timer(@roseGrowCycle - growing_period%@roseGrowCycle) do
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
    building.roseGrowedCount += 1
    send("rose_grow" + seperator + building.posID)
    building.roseGrowTimer = EM.add_timer(@roseGrowCycle) do
      RoseGrow(building)
    end
  end

  def roseGrowStop(building)
    EM.cancel_timer(building.roseGrowTimer)
    building.roseGrowTimer = nil
    building.roseGrowStopTimer = nil
    if building.roseGrowBeginTimeStamp == -1
      building.roseGrowLastDuration = building.roseGrowTotalDuration
    else
      building.roseGrowLastDuration = building.roseGrowTotalDuration - (Time.now - building.roseGrowBeginTimeStamp)
    end

  end

  def DownloadBuildings(protocal_no,contents)
    @userFile.Buildings.each{ |building|
      mazeLv = -1
      case building.type
        # 检查RoseBuilding是否该生成新的玫瑰
        when "Rose"
          RoseGrowBegin(building,Time.now - @userFile.logOutTimeStamp)
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
        when "Target"
          if building.isPvP
            targetFile = ReadPlayerFile(building.targetName+".yaml")
            mazeLv = targetFile.currentMazeLevel
          else
            mazeLv = GetPvEMazeLv(building.PvELevelIdx)
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
               @seperator + building.PvELevelIdx.to_s +
               @seperator + building.roseGrowLastDuration.to_s +
               @seperator + building.roseGrowTotalDuration.to_s +
               @seperator + building.bornNewTargetLastDuration.to_s +
      @seperator + mazeLv.to_s
               )
    }
    send("buildings_ready")
  end

  def DownloadClouds(protocal_no,contents)
    cloudprice = [5000,
                  5000,
                  5000,
                  12000,
                  5000,
                  7000,
                  7000,
                  9000,
                  12000]
    for i in 0..8
      cloud = @userFile.Clouds[i]
      price = cloudprice[i]
      send("one_cloud" +
               @seperator + cloud.idx.to_s +
               @seperator + price.to_s +
               @seperator + cloud.locked.to_s
      )
    end

    send("clouds_ready")

  end

  def DownloadReplays(protocal_no,contents)
    reply = protocal_no
    @userFile.defReplays.each{ |replay|
      send("replay" + @seperator + PackReplay(replay))
    }
    @userFile.atkReplays.each{ |replay|
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
        @seperator + replay.guard+
        @seperator + replay.reward_rose_count.to_s+
        @seperator + replay.rewardAccepted.to_s
    returnStr
  end

  def StealingOver(protocal_no,contents, enemy)
    replay = ReplayData.new()
    replay.date = contents[0]
    replay.StealingCash = contents[1]
    replay.ini = contents[2]
    replay.everClicked = contents[3]
    replay.thief = StealingInfo(@userFile)
    replay.reward_rose_count = 0
    replay.rewardAccepted = false
    bIsPerfectStealing = contents[4].to_bool
    enemyCashBeforeSteal = 1.0
    # 如果是pvp，要扣除对方金钱
    if !enemy.userFile.isBot
      replay.guard = StealingInfo(enemy.userFile)
      enemy.userFile.beenStealingTimeStamp = Time.now
      if replay.StealingCash.to_f < 1
        replay.reward_rose_count = 2
      end
      if $onlinePlayers.include?enemy
        enemy.send("been_stolen" + @seperator + PackReplay(replay) + @seperator + "True")
      else
        enemyCashBeforeSteal = enemy.userFile.cashAmount.to_f
        enemy.userFile.cashAmount = (enemyCashBeforeSteal - replay.StealingCash.to_f).to_s
        enemy.userFile.defReplays << replay
        enemy.userFile.defReplays.shift if enemy.userFile.defReplays.length > 5
        SaveToFile(enemy.userFile.name + ".yaml", enemy.userFile)
      end

    else
      # bot的金钱是客户端上传的
      enemy.userFile.cashAmount = contents[5].to_f
      enemyCashBeforeSteal = contents[5].to_f
      replay.guard = StealingInfo(enemy.userFile)
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
    building.unpickedRose = 1
    building.roseGrowedCount = 0
    building.type = "Rose"
    building.roseGrowBeginTimeStamp = Time.now
    RoseGrowBegin(building,0)
    send("new_rosebuilding" + @seperator + buildingPosID +
             @seperator + building.unpickedRose.to_s +
             @seperator + building.roseGrowLastDuration.to_s +
             @seperator + building.roseGrowTotalDuration.to_s)
  end

  def AdvanceTutorial(protocal_no,contents)
    @userFile.TutorialLevelIdx = contents[0]
  end

  def DownloadRanks(protocal_no,contents)
    players = []
    Dir.glob("*.yaml") do |player_achive_file|
      player = ReadPlayerFile(player_achive_file)
      players << player if player.TutorialLevelIdx == "Over"
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

  def CloudUnlock(protocal_no,contents)
    @userFile.Clouds[contents[0].to_i].locked = false
  end

  def BuildingUnlock(protocal_no,contents)
    SendNewTarget(GetBuilding(contents[0]))
  end

  def Close
    if(@userFile != nil)
      @userFile.Buildings.each{ |building|
        if building.roseGrowTimer != nil
          building.roseGrowLastDuration = building.roseGrowTotalDuration - (Time.now - building.roseGrowBeginTimeStamp)
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
      @userFile.logOutTimeStamp = Time.now
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
        player = Player.new(ws)
        $onlinePlayers << player
        enemy = nil
        ws.onmessage do |msg|
          begin
          contents = msg.to_s.split(player.seperator)
          puts "<#{sid}>" + contents.to_s
          protocal_no = contents[0]
          contents.shift
          case protocal_no
            when "login"
              player.Login(protocal_no,contents)
            when "self_stealing_info"
              player.SelfStealingInfo(protocal_no,contents)
            when "upgrade_maze"
              player.UpgradeMaze(protocal_no,contents)
            when "using_trick"
              player.UsingTrick(protocal_no,contents)
            when "unuse_trick"
              player.UnuseTrick(protocal_no,contents)
            when "add_trickitem"
              player.AddTrickItem(protocal_no,contents)
            when "consume_trickitem"
              player.ConsumeTrickItem(protocal_no,contents)
            when "drop_trickitem"
              player.DropTrickItem(protocal_no,contents,enemy)
            when "remove_droppedItem"
              if enemy != nil
                player.RemoveDroppedItem(protocal_no,contents,enemy)
              else
                player.RemoveDroppedItem(protocal_no,contents,player)
              end
            when "slot_bought"
              player.SlotBought(protocal_no,contents)
            when "learn_trick"
              player.LearnTrick(protocal_no,contents)
            when "download_buildings"
              player.DownloadBuildings(protocal_no,contents)
            when "download_clouds"
              player.DownloadClouds(protocal_no,contents)
            when "download_replays"
              player.DownloadReplays(protocal_no,contents)
            when "download_target"
              enemy = nil
              targetName = contents[0]
              isBot = contents[1].to_bool
              buildingPosID = contents[2]
              if !isBot
                $onlinePlayers.each{ |p|
                  if p.userFile.name == targetName.force_encoding('utf-8')
                    enemy = p
                    break
                  end
                }
                if enemy == nil
                  enemy = Player.new(ws)
                  enemy.userFile = enemy.ReadPlayerFile((targetName + ".yaml").force_encoding('utf-8'))
                end
                if enemy.userFile.cashAmount.to_f < 1000
                  enemy.userFile.cashAmount = "5000"
                end
              else
                enemy = Player.new(ws)
                enemy.userFile.name = targetName
                enemy.userFile.TutorialLevelIdx = "Over"
                enemy.userFile.isBot = true
                building = player.GetBuilding(buildingPosID)
                enemy.userFile.currentMazeRandSeedCache = building.botLevelRandSeed
                enemy.userFile.currentMazeLevel = player.GetPvEMazeLv(building.PvELevelIdx)
              end
              player.websocket.send("download_target" + player.seperator + enemy.StealingInfo(enemy.userFile))

            when "stealing_over"
              if enemy != nil
                player.StealingOver(protocal_no,contents, enemy)
                enemy = nil
              end
            when "cash"
              player.Cash(protocal_no,contents)
            when "pick_rose"
              player.PickRose(protocal_no,contents)
            when "add_safebox"
              player.AddSafebox(protocal_no,contents)
            when "upgrade_safebox"
              player.UpgradeSafebox(protocal_no,contents)
            when "hire_guard"
              player.HireGuard(protocal_no,contents)
            when "upload_summoned_guards"
              player.UploadSummonedGuards(protocal_no,contents)
            when "click_target"
              player.ClickTarget(protocal_no,contents)
            when "click_replay"
              player.ClickReplay(protocal_no,contents)
            when "reward_accepted"
              player.RewardAccepted(protocal_no,contents)
            when "poor_turn_to_rose"
              player.TurnToRoseBuilding(protocal_no,contents)
            when "advance_tutorial"
              player.AdvanceTutorial(protocal_no,contents)
            when "download_ranks"
              player.DownloadRanks(protocal_no,contents)
            when "download_other_replays"
              player.DownloadOtherReplays(protocal_no,contents)
            when "cloud_unlock"
              player.CloudUnlock(protocal_no,contents)
            when "building_unlock"
              player.BuildingUnlock(protocal_no,contents)
            when "change_rose"
              player.ChangeRose(protocal_no,contents)
            when "upload_magician"
              player.UploadMagician(protocal_no,contents)
            when "select_magician"
              player.SelectMagician(protocal_no,contents)
          end
          rescue Exception => e
            file = File.new('ErrorLogs.txt',"a+:UTF-8")
            file.write(player.userFile.name) if player.userFile != nil
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
        ws.onerror do|msg|
          puts msg
        end
        ws.onclose do
          begin
            $onlinePlayers.delete(player)
            player.Close
            puts "<#{sid}> disconnected"
            @channel.unsubscribe sid
            @channel.push "<#{sid}> disconnected"
          rescue Exception => e
            file = File.new('ErrorLogs.txt',"a+:UTF-8")
            file.write(player.userFile.name) if player.userFile != nil
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
          end

        end
      end
  end
end