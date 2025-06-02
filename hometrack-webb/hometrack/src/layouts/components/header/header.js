import classNames from "classnames/bind";
import style from "./header.module.scss";
import listSidebar from "../sidebar/listsidebar";

const cx = classNames.bind(style);

const list = listSidebar;

function Header() {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("content")}>{list.user}</div>
      <div className={cx("user")}>
        <div className={cx("admin")}>Admin</div>
        <div className={cx("item-admin")}>A</div>
      </div>
    </div>
  );
}

export default Header;
