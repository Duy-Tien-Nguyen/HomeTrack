import classNames from "classnames/bind";
import style from "./search.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';

const cx = classNames.bind(style);

function Search() {
  return (
    <div className={cx("wrapper")}>
      <div className={cx("main")}>
        <div className={cx("main-icon")}>
         
          <FontAwesomeIcon className={cx("icon")} icon={faMagnifyingGlass} />
        </div>
        <input className={cx("input")} placeholder="Tìm kiếm...  " />
      </div>
    </div>
  );
}

export default Search;
