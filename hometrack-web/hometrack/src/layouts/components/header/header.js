import classNames from "classnames/bind";
import style from "./header.module.scss";
import listSidebar from "../sidebar/listsidebar";
import Admin from "../admin/admin";

const cx = classNames.bind(style);

const list = listSidebar;

function Header() {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("content")}>{list.user}</div>
      <div className={cx("user")}>
        <Admin />
      </div>
    </div>
  );
}

export default Header;
