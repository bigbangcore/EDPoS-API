/*
 Source Server         : bigbang
 Source Server Type    : MySQL
 Source Server Version : 50729
 Source Schema         : bigbang_dpos

 Target Server Type    : MySQL
 Target Server Version : 50729
 File Encoding         : 65001

 Date: 07/05/2020 15:01:33
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for Block
-- ----------------------------
DROP TABLE IF EXISTS `Block`;
CREATE TABLE `Block`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `hash` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `fork_hash` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `prev_hash` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `time` bigint(20) NULL DEFAULT NULL,
  `height` int(11) NULL DEFAULT NULL,
  `type` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `reward_address` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '出块奖励地址',
  `reward_money` decimal(20, 10) NULL DEFAULT NULL COMMENT '出块奖励金额',
  `is_useful` bit(1) NULL DEFAULT b'1' COMMENT '是否有效，为最长链的区块',
  `bits` int(255) NULL DEFAULT NULL COMMENT '难度',
  `reward_state` bit(1) NULL DEFAULT b'0' COMMENT 'DPOS出块收益计算状态',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `hash`(`hash`) USING BTREE,
  INDEX `height`(`height`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for DposDailyReward
-- ----------------------------
DROP TABLE IF EXISTS `DposDailyReward`;
CREATE TABLE `DposDailyReward`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `dpos_addr` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT 'dpos节点地址',
  `client_addr` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '投票人地址',
  `payment_date` date NULL DEFAULT NULL COMMENT '收益日期',
  `payment_money` decimal(20, 10) NULL DEFAULT NULL COMMENT '当日总收益',
  `txid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '支付ID',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for DposRewardDetails
-- ----------------------------
DROP TABLE IF EXISTS `DposRewardDetails`;
CREATE TABLE `DposRewardDetails`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `dpos_addr` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点地址',
  `client_addr` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '投票人(第三方投票和节点自投)',
  `vote_amount` decimal(20, 10) NULL DEFAULT NULL COMMENT '投票金额',
  `reward_money` decimal(20, 10) NULL DEFAULT NULL COMMENT '投票收益',
  `reward_date` date NULL DEFAULT NULL COMMENT '收益日期',
  `block_height` int(11) NULL DEFAULT NULL COMMENT '区块高度',
  `reward_state` bit(1) NULL DEFAULT b'0' COMMENT '汇总状态，1表示已计算汇总，0表未计算汇总',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for PVInfo
-- ----------------------------
DROP TABLE IF EXISTS `PVInfo`;
CREATE TABLE `PVInfo`  (
  `pv_id` int(11) NOT NULL AUTO_INCREMENT,
  `pv_ip` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `pv_date` datetime(0) NULL DEFAULT NULL,
  `pv_page` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`pv_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for Pool
-- ----------------------------
DROP TABLE IF EXISTS `Pool`;
CREATE TABLE `Pool`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `address` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '地址',
  `name` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '矿池/节点名称',
  `type` varchar(10) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '类型(pow或dpos)',
  `key` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '节点调用的APIKey',
  `fee` decimal(20, 10) NULL DEFAULT NULL COMMENT '节点投票手续费率',
  PRIMARY KEY (`id`, `address`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for Task
-- ----------------------------
DROP TABLE IF EXISTS `Task`;
CREATE TABLE `Task`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `forkid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `block_hash` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `is_ok` bit(1) NULL DEFAULT b'0',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

-- ----------------------------
-- Table structure for Tx
-- ----------------------------
DROP TABLE IF EXISTS `Tx`;
CREATE TABLE `Tx`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `block_hash` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `txid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `form` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `to` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `amount` decimal(20, 10) NULL DEFAULT NULL,
  `free` decimal(20, 10) NULL DEFAULT NULL,
  `type` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `lock_until` int(11) NULL DEFAULT NULL,
  `n` tinyint(6) NULL DEFAULT NULL,
  `spend_txid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `data` varchar(4096) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `dpos_in` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '投票的dpos地址',
  `client_in` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '投票的客户地址',
  `dpos_out` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '赎回的dpos地址',
  `client_out` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '赎回的客户地址',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `block_id`(`block_hash`) USING BTREE,
  INDEX `txid`(`txid`) USING BTREE,
  INDEX `spend_txid`(`spend_txid`) USING BTREE,
  INDEX `to`(`to`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Compact;

SET FOREIGN_KEY_CHECKS = 1;
