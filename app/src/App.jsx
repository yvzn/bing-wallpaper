/*
   Copyright 2021-2022 Yvan Razafindramanana

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

import React from 'react';
import PropTypes from 'prop-types';

import getWallpapers, { numberOfImages } from './service';

const initialState = {
	images: new Array(numberOfImages).fill({ status: "loading" }),
};

function App() {
	const [state, setState] = React.useState(initialState);

	React.useEffect(() => {
		getWallpapers().then((newImages) => {
			setState((previousState) => {
				const images = newImages.map((image) => ({
					...image,
					status: "loaded",
				}));
				return {
					...previousState,
					images,
				};
			});
		});
	}, []);

	return <Wallpapers images={state.images} />;
}

export default App;

function Wallpapers({ images }) {
	return (
		<>
			{images.map((image, index) => (
				<WallpaperCard image={image} key={index} />
			))}
		</>
	);
}

const cachedImageProps = PropTypes.shape({
	status: PropTypes.oneOf(["loading", "loaded"]).isRequired,
	title: PropTypes.string,
	copyright: PropTypes.string,
	market: PropTypes.string,
	fullResolution: PropTypes.string,
	lowResolution: PropTypes.string,
});

Wallpapers.propTypes = {
	images: PropTypes.arrayOf(PropTypes.shape({
		image: cachedImageProps
	})).isRequired,
}

function WallpaperCard({ image }) {
	let lang = undefined;
	if (Boolean(image.market) && !image.market.startsWith('en')) {
		lang = image.market;
	}

	return (
		<article lang={lang}>
			<ImageTitle image={image} />
			<ImagePreview image={image} />
			<ImageCopyright image={image} />
		</article>
	);
}

WallpaperCard.propTypes = {
	image: cachedImageProps
}

function ImagePreview({ image }) {
	return (
		<p>
			{image.status === "loading" && <Skeleton />}
			{image.status === "loaded" && (
				<a href={image.fullResolution} rel="noreferrer noopener">
					<img
						src={image.lowResolution}
						alt={image.title}
						width="320"
						height="180"
					/>
				</a>
			)}
		</p>
	);
}

ImagePreview.propTypes = { image: cachedImageProps }

function ImageTitle({ image }) {
	let tooltip = Boolean(image.title) && image.title.length > 15 ? image.title : undefined;

	return (
		<h2>
			{image.status === "loading" && <Skeleton>Loading...</Skeleton>}
			{image.status === "loaded" && (
				<a href={image.fullResolution} rel="noreferrer noopener" title={tooltip} tabIndex={-1}>{image.title}</a>
			)}
		</h2>
	);
}

ImageTitle.propTypes = { image: cachedImageProps }

function ImageCopyright({ image }) {
	return image.status === "loaded" && <footer>{image.copyright}</footer>;
}

ImageCopyright.propTypes = { image: cachedImageProps }

function Skeleton({ children }) {
	return <progress max="1">{children}</progress>;
}

Skeleton.propTypes = {
	children: PropTypes.oneOfType([PropTypes.string, PropTypes.element]),
};
