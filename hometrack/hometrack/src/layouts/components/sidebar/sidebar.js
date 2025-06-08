import classNames from "classnames/bind";
import style from "./sidebar.module.scss";
import listSidebar from "./listsidebar";
import { Link } from "react-router-dom";

import Content from "../../../pages/content";
import Dashboard from "../../../pages/dashboard";
import Monitoring from "../../../pages/monitoring";
import User from "../../../pages/user";
import System from "../../../pages/system";



const cx = classNames.bind(style);
const list = listSidebar;
function Sidebar() {
  return (
    <div className={cx("wrapper")}>
      <div className={cx('sidebar-content')} >
        <Link className={cx('content')} to="/user">{list.user}</Link>
       
      </div>
      <div className={cx('sidebar-content')} >
        <Link className={cx('content')} to="/system">{list.system}</Link>
      </div>
      <div className={cx('sidebar-content')} >
        <Link className={cx('content')} to="/monitoring">{list.monitoring}</Link>
      </div>
      <div className={cx('sidebar-content')} >
        <Link className={cx('content')} to="/content">{list.content}</Link>
      </div>
    </div>
  );
}

export default Sidebar;
