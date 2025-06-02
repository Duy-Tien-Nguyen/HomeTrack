import classNames from "classnames/bind";
import style from "./information.module.scss";

const cx = classNames.bind(style);

function Information() {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("content")}>
        <div className={cx("user")}>A</div>
      </div>
    </div>
  );
}

export default Information;
