import classNames from "classnames/bind";
//
import style from "./defaultLayout.module.scss";
import Header from "../components/header/header";
import Sidebar from "../components/sidebar/sidebar";
import content from "../components/sidebar/content/content";
import section from "../components/sidebar/section/section";

const cx = classNames.bind(style);

function DefaultLayout({ children }) {
  return (
    <div className={cx("wrapper")}>
      <Header />
      <div className={cx("container")}>
        <div className={cx("side-bar")}>
          <Sidebar />
        </div>
        <div className={cx("content")}>{children}</div>
      </div>
    </div>
  );
}

export default DefaultLayout;
