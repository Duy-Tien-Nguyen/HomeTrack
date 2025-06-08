import classNames from "classnames/bind";
import style from "./popperAdmin.module.scss";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRightFromBracket } from "@fortawesome/free-solid-svg-icons";


const cx = classNames.bind(style);


function PopperAdmin() {
    return ( <div className={cx('wrapper')}>
       <div className={cx('item')}>
        <div className={cx('icon')}>
            <FontAwesomeIcon className={cx('icon-c')} icon={faRightFromBracket}  />
        </div> 
        <div className={cx('content')}>Đăng xuất</div>
        </div>
    </div> );
}

export default PopperAdmin;

