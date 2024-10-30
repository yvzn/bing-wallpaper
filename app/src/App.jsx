/*
   Copyright 2021-2024 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

import React from "react";
import PropTypes from "prop-types";

import getWallpapers, { numberOfImages } from "./service";

const initialState = {
	images: new Array(numberOfImages).fill({}),
	status: "loading",
	selectedImage: undefined,
};

function App() {
	const [state, setState] = React.useState(initialState);

	React.useEffect(() => {
		getWallpapers().then((images) => {
			setState((previousState) => {
				return {
					...previousState,
					images,
					status: "loaded",
				};
			});
		});
	}, []);

	const selectImage = (image) => {
		console.log("Selected image:", image);
		setState((previousState) => {
			return {
				...previousState,
				selectedImage: image,
			};
		});
	};

	return (
		<>
			<Wallpapers
				images={state.images}
				status={state.status}
				onSelectImage={selectImage}
			/>
		</>
	);
}

export default App;

function Wallpapers({ images, status, onSelectImage }) {
	const selectWallpaperCard = (index) => () => {
		onSelectImage(index);
	};

	return (
		<>
			{images.map((image, index) => (
				<WallpaperCard
					key={index}
					image={image}
					status={status}
					onClick={selectWallpaperCard(index)}
				/>
			))}
		</>
	);
}

const cachedImageProps = PropTypes.shape({
	title: PropTypes.string,
	copyright: PropTypes.string,
	market: PropTypes.string,
	fullResolution: PropTypes.string,
	lowResolution: PropTypes.string,
});

const loadingStatus = PropTypes.oneOf(["loading", "loaded"]);

Wallpapers.propTypes = {
	images: PropTypes.arrayOf(
		PropTypes.shape({
			image: cachedImageProps,
		}),
	).isRequired,
	status: loadingStatus.isRequired,
	onSelectImage: PropTypes.func.isRequired,
};

function WallpaperCard({ image, status, onClick }) {
	let lang = undefined;
	if (Boolean(image.market) && !image.market.startsWith("en")) {
		lang = image.market;
	}

	return (
		<article lang={lang}>
			<ImageTitle image={image} status={status} />
			<ImagePreview image={image} status={status} onClick={onClick} />
		</article>
	);
}

WallpaperCard.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
	onClick: PropTypes.func.isRequired,
};

function ImagePreview({ image, status, onClick }) {
	return (
		<p>
			{status === "loading" && <Skeleton />}
			{status === "loaded" && (
				<button type="button" onClick={onClick}>
					<img
						src={image.lowResolution}
						alt={image.title}
						width="320"
						height="180"
					/>
				</button>
			)}
		</p>
	);
}

ImagePreview.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
	onClick: PropTypes.func.isRequired,
};

function ImageTitle({ image, status }) {
	let tooltip =
		Boolean(image.title) && image.title.length > 15
			? image.title
			: undefined;

	return (
		<h2>
			{status === "loading" && <Skeleton>Loading...</Skeleton>}
			{status === "loaded" && <b title={tooltip}>{image.title}</b>}
		</h2>
	);
}

ImageTitle.propTypes = {
	image: cachedImageProps,
	status: loadingStatus.isRequired,
};

function Skeleton({ children }) {
	return <progress max="1">{children}</progress>;
}

Skeleton.propTypes = {
	children: PropTypes.oneOfType([PropTypes.string, PropTypes.element]),
};
